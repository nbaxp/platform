# Fetch 网络请求

## fetch 异常处理

```javascript
async function request(url, options) {
  try {
    //before request
    const response = await fetch(url, options)
    //after request
    const result = null
    const contentType = response.headers.get('Content-Type')
    result = response.json()
    return result
  } catch (error) {
    if (error.name === 'AbortError') {
      console.log(error.name) //AbortError
    } else {
      console.log(error.name) //TypeError
    }
    return { code: error.name, message: error.message, data: error }
  }
}
await request('')
```
