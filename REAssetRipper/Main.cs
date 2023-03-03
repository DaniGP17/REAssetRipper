using REAssetRipper.Styles;
using REAssetRipper.Constants;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Button testingTitle = new Button(1, "Extract Options");
Button testingTitle2 = new Button(1, "Import to Unreal Engine");
Button testingTitle3 = new Button(1, "Options");
Console.WriteLine(testingTitle.GetButton());
Console.WriteLine(testingTitle2.GetButton());
Console.WriteLine(testingTitle3.GetButton());