---
layout:
  title:
    visible: true
  description:
    visible: false
  tableOfContents:
    visible: true
  outline:
    visible: true
  pagination:
    visible: false
---

# Setup

### Install UniFlux

* You can use the _.unityPackage_ in releases
* You can use the \*.tzg in releases and add in PackageManager
* You can add in PackageManager ([How to install package from git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html))

```bash
https://github.com/xavierarpa/UniFlux.git
```

* You can install via openupm CLI

```bash
openupm add com.xavierarpa.uniflux
```

* You can install via npm

```bash
npm i com.xavierarpa.uniflux
```

### Generate your Scritps

{% code lineNumbers="true" fullWidth="true" %}
```csharp
using UniFlux;
using UnityEngine;

public class Test : MonoFlux
{
    private void Start()
    {
        // Allows you to call any method subscribe with "0" Key
        "0".Dispatch();
    }
    [MethodFlux("0")] private void Tester()
    {
        Debug.Log("Hello world");
    }
}
```
{% endcode %}

{% code fullWidth="true" %}
```csharp
using UniFlux;
using UnityEngine;

public class Test_2 : MonoFlux
{
    [MethodFlux("0")] private void Tester_2()
    {
        Debug.Log("Hello world 2");
    }
}
```
{% endcode %}

You can put both in a scene and see if they both receive the event

That's all !
