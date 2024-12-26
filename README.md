
# Summary

This is a testing project where we will show how to Unit Test plugin development in D365 CE.

## What is Unit Testing and Why should I do it?

Unit Testing in the context of D365 CE Plugin Development refers to the process of testing 
individual components of a plugin's business logic in isolation from the Dynamics 365 
environment. This involves simulating the Dynamics platform using frameworks like FakeXrmEasy 
to ensure that the plugin behaves as expected under different conditions.

**Key Aspects of Unit Testing**

1. Isolation:  
    Tests focus only on the plugin's logic, excluding dependencies like the actual CRM server, network calls, or external services.

2. Automation:  
    Tests are automated, providing quick feedback during development.

3. Determinism:  
    Tests produce consistent results when given the same inputs, regardless of the environment.

4. Validation Scope:  
    Business rule enforcement (e.g., required fields)
    Data validation (e.g., format checks)
    Correct plugin responses to create, update, or delete messages

5. Mocked Environment:  
    FakeXrmEasy provides a fully simulated CRM environment, including entities, plugins,
    and workflows. This avoids dependencies on live Dynamics instances.  
    
## Setting up the Environment

### Tools Required

- [Visual Studio](https://visualstudio.microsoft.com/downloads/) (We will be using 2022)

### Project Setup

We won't go into details in how to setup a Plugins Project as that should be common knowledge.
That said, we recommend using the [Power Platform CLI](https://learn.microsoft.com/en-us/power-platform/developer/cli/introduction?tabs=windows)
(PAC) to create new plugins using the command

``` powershell
pac plugin init -o {PluginsProjectFolder}
```

We will be using [XUnit Project Template](https://marketplace.visualstudio.com/items?itemName=jsakamoto.xUnitTestProjectTemplate)
from the Visual Studio Marketplace which targets *.net framework 4.7.2* and comes with
*XUnit* already installed

#### **Used Libraries**

- [XUnit](https://xunit.net/) as the testing engine
- [Moq](https://www.nuget.org/packages/Moq) for mocking service dependencies
- [FluentAssertions](https://fluentassertions.com/) for improved test readability
- [FakeXrmEasy Nuget package](https://www.nuget.org/packages/FakeXrmEasy.v9/2.5.0) for faking D365 CE pipeline 

#### **Steps to Create the Test Project**

1. Add a new Project to you solution using the previously mentioned template
2. Add a reference to you Plugin Project(s)
4. Rename the file created by default: `UnitTest1.cs` to `SanityTests.cs`
5. Inside said file rename the method `TestMethod1` to `SanityCheck`  
6. Run all tests  
   This ensures that there's nothing wrong with the way XUnit is properly configured and we're 
   off to a good start, also, if we have issues while running the tests as part of some 
   automation **(Highly Recommended)** we can quickly rule out package misconfiguration
7. Install Nuget Packages
    - [Moq](https://www.nuget.org/packages/Moq)
    - [FluentAssertions](https://www.nuget.org/packages/FluentAssertions/7.0.0) 
    - [FakeXrmEasy.v9](https://www.nuget.org/packages/FakeXrmEasy.v9/2.5.0)  

        >  **Note:** Be mindful of the *FakeXrmEasy* version you install:  
        >    - **3.x.x** are for client-side applications written for the new *.net6* and newer
        >    - **2.x.x** are for server-side applications (plugins) written for 
        >                *.net framework*  
        >
        >  We will be using version **2.5.0** which the latest at the time.

**Note:** DO NOT update the package versions for the packages that come with the project: 
 `xunit`, `coverlet.collector`, `Microsoft.Net.Test.Sdk` and `xunit.runner.visualstudio` as 
 they will break your project due to some incompatibilities
