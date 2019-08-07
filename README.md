UtilKits
=======

|NuGet 名稱|版本|說明|
|---|---|---|
|UtilKits|[![NuGet](https://img.shields.io/nuget/v/UtilKits.svg)](https://www.nuget.org/packages/UtilKits/)| 暫存、資料庫、API、額外擴充函式|
|UtilKits.Configuration|[![NuGet](https://img.shields.io/nuget/v/UtilKits.Configuration.svg)](https://www.nuget.org/packages/UtilKits.Configuration/)|.NET Core Configuration 靜態函式|
|UtilKits.Crypto|[![NuGet](https://img.shields.io/nuget/v/UtilKits.Crypto.svg)](https://www.nuget.org/packages/UtilKits.Crypto/)|加解密套件、自訂定加解密 (Private Repository)|

## Overview

支援 .NET Core 2.0 以上 及 .NET Framework 4.6 以上

將常用的模組、函式套件集結為一個Library，讓 C# 專案上開發更為快速

## Table Of Contents
* [Installation](#installaction)
* [Usage](#usage)
  * [Cache](#cache)
  * [Client](#client)
  * [Database](#database)
  * [Extension](#extension)
  * [Refection](#refection)
  * [Serilizer](#serilizer)
  * [Configuration](#configuration)
  * [Crypto](#crypto)
* [License]

## Installation

UtilKits 是公開可用的 NuGet 封裝程式，你可以透過 Visual Studio 介面安裝或使用下列的指令安裝：
```
PM> Install-Package UtilKits
```
若使用 **.NET Core** 時，想要用早期的方式存取 Configuration 則可以安裝：
```
PM> Install-Package UtilKits.Configuration
```
`Configuration 專案相依於 Crypto 專案`

若有客制化加解密的需求，則可安裝
```
PM> Install-Package UtilKits.Crypto
```

## Usage
共用程式集內包含了許多處理資料的函數及模組，在下列的文件內容會說明每個用法。

## Cache
使用模組的方式去設定或取得暫存內的資料。

``` C#
// ProductModelCache.cs
// 建置 ProductModel 的暫存模組
public class ProductModelCache : CacheFactory<ProductModel>
{
    public ProductModelCache(int productId) : base($"Product_{productId}")
}

// ProductModule.cs
// 商品模組
public class ProductModule
{
    public ProductModel Get(int productId)
    {
        // 設立商品暫存模組
        var cache = new ProductModelCache(productId);
        // 取得商品的資料
        var product = cache.Get();
        // 回傳資料
        return product;
    }

    public void Save(ProductModel model)
    {
        // 寫入商品資料至資料庫或檔案
        // …
        
        var cache = new ProductModelCache(model.ProductId);
        // 將更改的資料存回暫存
        cache.Set(model); 
    }
}
```

### Advanced Cache
當每次取暫存時，希望先檢查暫存內是否有資料，有則回傳暫存結果，無則再去資料庫取資料暫存後，再回傳結果。

``` C#
public ProductModel Get(int productId)
{
    var cache = new ProductModelCache(productId);
    var product = cache.Get(() => {
        // 當沒暫存內無資料時，才會執行此函式
        
        var product = /* 從資料庫取得資料 */

        // 至資料庫取得商品內容，並寫入暫存
        return product;
    });
}
```

### Dependency Cache
當主要的暫存內容變動後，相依的暫存會在下次取用時更新。

``` C#
// 主要暫存內容
public ProductModel Get(int productId)
{
    var cache = new ProductModelCache(productId);
    var product = cache.Get(() => {
        var product = /* 從資料庫取得資料 */
        // 至資料庫取得商品內容，並寫入暫存
        return product;
    });
}

// 相依暫存內容
public ProductCompaignModel Get(int productId)
{
    var productCache = new ProductModelCache(productId);
    var compaginCache = new ProductCompaignModelCache(productId);

    var compaign = compaginCache.Get(() => {
        var product = productCache.Get();
        
        return // 從商品資訊內容去計算折價的結果 
    },() => {
        // 在取用相依暫存時，檢查主要暫存內容的時間是否有重新設定
        // 有：刪除相依暫存，重取內容，否：回傳相依暫存內容
        return compaignCache.OverDependencyTime(productCache)
    });

}
```

### Paging Cache
分頁專用的暫存模組，當刪除掉其中一頁時，同時會刪除其他的頁面
``` C#
// CategoryProductModelCache.cs
// 分頁暫存模組
public class CategoryProductModelCache : CacheFactory<List<CategoryProductModel>>
{
    public CategoryProductModelCache(int categoryId, int pageIndex, int pageSize) 
        : base($"CategoryProduct_{categoryId}", pageIndex, pageSize) 
    { }
}

// CateogryModule.cs
// 取得分頁內容
public List<CategoryProductModel> GetList(int categoryId, int pageIndex, int pageSize)
{
    var cache = new CategoryProductModelCache(categoryId, pageIndex, pageSize);
    var result = cache.Get(() => /* 取得分頁資料內容 */ );

    return result;
}

public void ClearList(int categoryId)
{
    var cache = new CategoryProductModelCache(categoryId, /* 任意數 */, /* 任意數 */);
    cache.Delete();
}
```

## Client
後台程式需要使用到第三方的API或Form-Submit時，可以將此接口藉由繼承模組化。

並在傳送前可將model直接序列化為指定的格式，回傳後可反序列化為自訂義的model。
``` C#
public class PaymentService : ApiClient
{
    public PaymentService: base() { }

    public bool Checkout(OrderDto order)
    { 
        /* STEP1 取得 Token */ 
        var tokenUri = new Uri("[Domain]/api/token");
        var requestModel = new TokenRequestModel { /* 初始需求資訊 */ };
        var tokenResponse = Post<TokenResponseModel>(tokenUri, requestModel);

        /* STEP2 發送金流結帳 */
        var checkoutUri = new Uri("[Domain]/api/checkout");
        this.HttpHeader.Add('Authorization', tokenResponse.Token)
        var checkoutResponse = Post(checkoutUri, model);

        /* 處理回傳的結果 …*/

        return checkoutResponse.StatusCode == HttpStatusCode.OK;

    }
}
```
可以在模組建構時，決定傳入的 Request 及 傳回的 Response 各是什麼格式

|類型|JSON|XML|INI|Form Url Encord|Multipart|
|---|---|---|---|---|---|
|Request|○|○|○|○|○|
|Response|○|○|○|✕|✕| 

於程式建構時帶入，若未帶入預設皆為 JSON
``` C#
public class PaymentService : ApiClient
{
    public PaymentService: base(
        SerializerRequestFormat.Json, 
        SerializerResponseFormat.Xml) { }
}
```

### Client Class
提供常用的四種 Method 動作： GET, POST, PUT, DELETE

並依呼叫的函式決定是否需要即時反序列化回傳內容，下列為可使用的參數或函數方法
``` C#
// 回傳結果
public string ResponseBody
// 回傳狀態碼
public HttpStatusCode StatusCode
// 限制最大傳送大小
public long MaxResponseContentBufferSize
// API Timeout 時間
public TimeSpan Timeout
// The Media Type Header Value
public MediaTypeWithQualityHeaderValue MediaTypeHeaderValue
// Gets or sets the http header
public Dictionary<string, string> HttpHeader

// 格式化類型(Request)
protected SerializerRequestFormat _requestFormat;
// 格式化類型(Response)
protected SerializerResponseFormat _responseFormat;

// 將 GET 要求傳送至指定的 URI
protected TResponse Get<TResponse>(Uri uri);

// 將 POST 要求傳送至指定的 URI (不含內容)
protected TResponse Post<TResponse>(Uri uri);
// 將 POST 要求傳送至指定的 URI
protected TResponse Post<TResponse>(Uri uri, string content);
// 將 POST 要求傳送至指定的 URI
protected TResponse Post<TRequest, TResponse>(Uri uri, TRequest request);
// 將 POST 要求傳送至指定的 URI
protected TResponse Post<TResponse>(Uri uri, HttpContent httpContent);
// 將 POST 要求傳送至指定的 URI，不反序列化回傳內容
protected bool Post<TRequest>(Uri uri, TRequest request);

// 將 PUT 要求傳送至指定的 URI
protected TResponse Put<TRequest, TResponse>(Uri uri, TRequest request);
// 將 PUT 要求傳送至指定的 URI
protected TResponse Put<TResponse>(Uri uri, HttpContent httpContent);
// 將 PUT 要求傳送至指定的 URI，不反序列化回傳內容
protected bool Put<TRequest>(Uri uri, TRequest request);

// 將 DELETE 要求傳送至指定的 URI
protected TResponse Delete<TResponse>(Uri uri, HttpContent httpContent);
// 將 DELETE 要求傳送至指定的 URI
protected bool Delete(Uri uri, HttpContent httpContent);

// 將 HttpContent 要求傳送至指定的 URI
protected virtual TResponse Send<TResponse>(Func<HttpClient, HttpResponseMessage> action);
// 將 HttpContent 要求傳送至指定的 URI，不反序列化回傳內容
protected virtual bool Send(Func<HttpClient, HttpResponseMessage> action);
```

### Advanced Client

Send 函式為所有動作送出的最後接口，可 override 該函式，也可自行客制化自己的發送方式

使用 override ，在每次Request之前取得Token，並建置於Request標頭
``` C#
// PaymentTokenService.cs
public class PaymentTokenService : ApiClient
{
    public PaymentTokenService: base() { }

    public TokenResponseModel GenerateToken(TokenRequestModel model)
    {
        var tokenUri = new Uri("[Domain]/api/token");
        return Post<TokenResponseModel>(tokenUri, requestModel);
    }
}

// PaymentService.cs
public class PaymentService : ApiClient
{
    public PaymentService: base() { }

    protected overried bool Send(Func<HttpClient, HttpResponseMessage> action)
    {
        /* STEP1 取得 Token */ 
        var tokenService = new PaymentTokenService();
        var tokenResponse = tokenService.GenerateToken(/* 初始需求資訊 */ );

        this.HttpHeader.Add('Authorization', tokenResponse.Token);

        return base.Send(action);
    }

    public bool Checkout(OrderDto order)
    { 
        /* STEP2 發送金流結帳 */
        var checkoutUri = new Uri("[Domain]/api/checkout");
        var checkoutResponse = Post(checkoutUri, model);

        /* 處理回傳的結果 …*/

        return checkoutResponse.StatusCode == HttpStatusCode.OK;

    }
}

```
