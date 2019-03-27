# Implementation

Build your exception middleware and provide the following implementation of the AzureErrorLog, to enable registration of error and exceptions directly into Azure Table storage.

1) Build your model class by inheriting AzureErrorLogEntry.

```
internal class ErrorLogEntry : AzureErrorLogEntry {
  public string Category { get; set; }
  public string ErrorMessage { get; set; }
}
```

2) Build your implementation class by inheriting AzureErrorLog.

```
internal class ErrorLogImplementation : AzureErrorLog {
  internal ErrorLogImplementation() : base(new ErrorLogSettings()) { 
    ...
  }
}
```

3) In your exception middleware, trace errors as follows:

```
errorLog.Trace<ErrorLogEntry>(...
```
