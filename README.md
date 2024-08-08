# Intent.Samples.MudBlazor

Welcome to the **Intent.Samples.MudBlazor** repository! This sample project showcases the front-end design capabilities introduced in the Intent Architect v4.3.* beta release.

## Getting Started

Follow these steps to explore the sample:

1. **Download Intent Architect 4.3.* beta**  
   If you haven't already, [download](https://intentarchitect.com/#/downloads) and install the Intent Architect 4.3.* beta. Don’t worry—it can be installed side-by-side with your current version of Intent Architect.

2. **Clone the Repository**  
   Clone this repository to your local machine.

3. **Open the Solution in Intent Architect**  
   Navigate to the `intent` directory and open the `MudBlazor.ExampleApp.isln` solution file.

4. **Run the Software Factory**  
   Execute the `Software Factory` to generate the necessary code and configurations.

5. **Open the Generated Solution**  
   Once generated, open the resulting solution in your preferred IDE (e.g., Visual Studio).

6. **Configure the Startup Project**  
   Set your startup project to `MudBlazor.ExampleApp.Api` and ensure the Launch Profile is set to `MudBlazor.ExampleApp.Client`.

7. **Implemenet Code Implementations (optional)**
   Please see code implementation instructions below. These are areas that we aren't able to automate (yet) and so need to implement these by hand.

8. **Run the Solution**  
   Now, you're ready to run the solution and see the new front-end design features in action!

## Learn More

For a comprehensive overview of what's included in this release, take a look at the [4.3 release notes](https://docs.intentarchitect.com/articles/release-notes/intent-architect-v4.3.html).

## Code Implementations

To get the adding and removing of Invoice Lines on the Add Invoice form, you will need to implement the `AddLineClick()` and `OnDeleteLineClick(...)` methods in the `InvoiceForm.razor.cs` file (under `MudBlazor.ExampleApp.Client/Pages/Invoices/Components/InvoiceForm.razor`) as follows:

```csharp
private void AddLineClick()
{
   // [IntentIgnore]
   Model.OrderLines.Add(new InvoiceInvoiceLineDto());
}

private void OnDeleteLineClick(InvoiceInvoiceLineDto orderLine)
{
   // [IntentIgnore]
   Model.OrderLines.Remove(orderLine);
}

```