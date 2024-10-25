# 🧪 Bezier Curve – Technical Assessment

This repository showcases **unit tests for Bézier curve functionality** as part of a **technical assessment**. Written in **C#**, these tests validate key curve properties and methods using the **NUnit** framework, ensuring reliable results for various transformations, arc-length calculations, and more.

---

## 🚀 Setup & Installation  

Follow these steps to run the tests locally:

### 1️⃣ Install NUnit  
Open the **Developer PowerShell** in Visual Studio and run:  
```bash
Install-Package Microsoft.NET.Test.Sdk -Version 17.11.1
Install-Package MSTest.TestAdapter -Version 3.6.1
Install-Package MSTest.TestFramework -Version 3.6.1
```

### 2️⃣ Add Dependencies to the BezierTests Project

- Right-click on the project in **Solution Explorer**.  
- Select **Manage NuGet Packages**, search for **NUnit**, and install **v3.13.3**.

### 3️⃣ Verify Additional Dependencies  
Ensure the following packages are installed:  
```
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
<PackageReference Include="MSTest.TestAdapter" Version="3.6.1" />
<PackageReference Include="MSTest.TestFramework" Version="3.6.1" />
```
## 🛠 Usage  
Make sure to set the **BezierCurve** project as the **startup project** in Visual Studio to run the tests successfully.

For official documentation on how to run unit tests, check [this link](https://learn.microsoft.com/en-us/visualstudio/test/getting-started-with-unit-testing?view=vs-2022&tabs=dotnet%2Cmstest#run-unit-tests).

## 🧩 What's Included?  
This project tests various functionalities of **Bézier curves**, including:

✅ **Start and end points**  
✅ **Tangent and normal vectors**  
✅ **Curve length and bounding box**  
✅ **Splitting and clipping operations**  
✅ **Arc-length and closest point calculations**  

Feel free to explore, run the tests, and reach out if you have any questions! 🎯
