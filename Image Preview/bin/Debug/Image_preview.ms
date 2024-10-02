


try(dotnet.loadassembly  (getFilenamePath  (getThisScriptFilename())+@"\Image Preview.dll") )catch() 	
----------------------------------
	
rollout imageloader "small browser" width:240 height:270
(	
	dotNetControl small_dialog "Image_Preview.UserControl1"  pos:[0,0] width:480 height:480
	

	
	
	
	
	on imageloader open do
   (
 showproperties small_dialog
	  
	   --showEvents  small_dialog
	
	small_dialog.Filepath=@"C:\MyAssets\Model library\bed\bed cover pillow\"
	small_dialog.Reload()
	small_dialog.Select()	   
	   
   )
   
   edittext extentions "Extentions:" text:".jpg|.png|"
   button b01 "Refresh"
   on b01 pressed do 
   (
	   small_dialog.extentions = extentions.text
	   small_dialog.Reload()
   )
   
   button b02 "Select directory"
   on b02 pressed do
   (
	   small_dialog.extentions = extentions.text
	   mypath = thePath = getSavepath()
	   
	   small_dialog.DirectoryLoad mypath
	   print mypath
		
   )
	button b03 "PrintName"
	   on b03 pressed do
	   (
		   print "Saiyedul"
		   --print small_dialog.MyName1
		   
		   --small_dialog.ShowImageDetails @"C:\Users\mdsai\OneDrive\Desktop\errr.png"
		  
		   
	   )
  --on small_dialog thumbpicked sender args do
		--( print small_dialog.currentitem
		--print args.FileName
		--print args.Image
		--)
   
)

rollout imageloader2 "small browser2" width:240 height:270
(	
	
	dotNetControl image_method "Image_Preview.UserControl2" pos:[0,0] width:480 height:480
	dotNetControl user_control3 "Image_Preview.UserControl3" pos:[0,0] width:480 height:480
	

	
	
	
	
	on imageloader open do
   (
 showproperties small_dialog
	  
	   --showEvents  small_dialog
	
	small_dialog.Filepath=@"C:\MyAssets\Model library\bed\bed cover pillow\"
	  --small_dialog.Reload()
	small_dialog.Select()	   
	   
   )
   
   edittext extentions "Extentions:" text:".jpg|.png|.Max"
   button b01 "Refresh"
   on b01 pressed do 
   (
	   small_dialog.extentions = extentions.text
	   small_dialog.Reload()
   )
   
   button b02 "Select directory"
   on b02 pressed do
   (
	   small_dialog.extentions = extentions.text
	   mypath = thePath = getSavepath()
	   small_dialog.Filepath = mypath
	   small_dialog.Reload()
   )
	button b03 "PrintName"
	   on b03 pressed do
	   (
		   print "Saiyedul"
		   print small_dialog.MyName1
		   
		   --small_dialog.ShowImageDetails @"C:\Users\mdsai\OneDrive\Desktop\errr.png"
		   image_method.ImageSize @"C:\Users\mdsai\OneDrive\Desktop\errr.png"
		   print "Image_Methods"
		   showMethods image_method
		   print "UserControl3"
		   showMethods user_control3
		   print user_control3.Orko
		   
	   )
  on small_dialog thumbpicked sender args do
		( print small_dialog.currentitem
		print args.FileName
		print args.Image
		)
   
)


imagedialog = newRolloutFloater "small browser" 500 600
addRollout  imageloader imagedialog
addRollout  imageloader imagedialog

