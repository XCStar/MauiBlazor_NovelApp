
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Storage;
using System.IO;
namespace MauiApp3.Views;
#if ANDROID
using Com.Benjaminwan.Ocrlibrary;
using Microsoft.Maui.Graphics.Platform;
#endif
public partial class OCR : ContentPage
{
    
    private readonly IFilePicker filePicker;
    public string BtnText { get; set; } = "识别";
    public string Text { get; set; } = "";
#if ANDROID
    private readonly OcrEngine ocrEngine;
     public OCR(OcrEngine ocrEngine,IFilePicker filePicker)
	{
		 InitializeComponent();
         this.ocrEngine=ocrEngine;
         BindingContext = this;
         this.filePicker = filePicker;
    }

#else

    public OCR(IFilePicker filePicker)
	{
		 InitializeComponent();
         BindingContext = this;
        this.filePicker = filePicker;

    }
#endif
   

    private async void Button_Clicked(object sender, EventArgs e)
    {


#if ANDROID
      //Detect (global::Android.Graphics.Bitmap input, global::Android.Graphics.Bitmap output, int padding, int maxSideLen, float boxScoreThresh, float boxThresh, float unClipRatio, bool doAngle, bool mostAngle)
       //ocrEngine.Detect(null,null,50,0,0.3,2,0.6,true,true);
        var fileResult= await filePicker.PickAsync(new PickOptions { PickerTitle="识别文件"  });
  
        if (fileResult!=null)
        {
                 
                  recText.Text="正在识别中..........";
                  //using var readStream=  await fileResult.OpenReadAsync();
                  //var ms= Stream2MemoryStream(readStream);
                  //var bytes=ms.ToArray();
                  //var src= Android.Graphics.BitmapFactory.DecodeByteArray(bytes,0,bytes.Length);
                  ocrResultImg.Source=ImageSource.FromStream(()=>fileResult.OpenReadAsync().Result);
                  OcrResult ocrResult=default(OcrResult);
                  Android.Graphics.Bitmap bgBitmap=default(Android.Graphics.Bitmap);
                  await Task.Run(()=>{
                      var sourceBitmap=Android.Graphics.BitmapFactory.DecodeFile(fileResult.FullPath);
                      bgBitmap=Android.Graphics.Bitmap.CreateBitmap(sourceBitmap.Width,sourceBitmap.Height,Android.Graphics.Bitmap.Config.Argb8888);
                      var maxSizeLen=Math.Max(sourceBitmap.Width,sourceBitmap.Height)*13;
                       ocrResult=ocrEngine.Detect(sourceBitmap,bgBitmap,maxSizeLen);
                  
                  });
                  if(ocrResult!=null)
                  {
                      if(string.IsNullOrEmpty(ocrResult.StrRes))
                      {
                         recText.Text="无识别结果";
                      }
                      else
                      {
                        recText.Text=ocrResult.StrRes;
                      }
                      Text=ocrResult.StrRes;
                      if(bgBitmap!=null)
                      {
                            var path= FileSystem.Current.CacheDirectory;
                            var cachePath= Path.Combine(path,"ocrcache");
                            if(!Directory.Exists(cachePath))
                            {
                               Directory.CreateDirectory(cachePath);
                            }
                            var fileName=Path.Combine(cachePath,"ocr.png");
                            if(File.Exists(fileName))
                            {
                              File.Delete(fileName);
                            }
                          using(var stream=File.Open(fileName,FileMode.Create,FileAccess.Write))
                          {
                            await bgBitmap.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Png,100,stream);
                           
                          }
                          ocrResultImg.Source=ImageSource.FromFile(fileName);
                      }
                      
                  }
                  else
                  {
                       recText.Text="无识别结果";
                  }
                 
                
                  
                  
        }
      
      
#endif
    }

    public MemoryStream Stream2MemoryStream(Stream stream)
    {

        var memoryStream = new MemoryStream();
        var buffer = new byte[4096];
        var len= stream.Read(buffer);
        while (len > 0)
        {
            memoryStream.Write(buffer,0,len);
            len = stream.Read(buffer);
        }
        return memoryStream;
    }

    private async void Copy_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Text))
        {
           await Clipboard.Default.SetTextAsync(null);
           await  Clipboard.Default.SetTextAsync(Text);
           await DisplayAlert("提示", "复制成功！", "确认");
        }
    }
}
