   M S B A M L   `   `   `  ����F  BIOPT-Client, Version=1.0.1.2, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.WDashbordCreate   5   	   U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	     � Title$   ��5   	   $	��334��6   $	��562��6    � 
ResizeMode$ NoResize=�6+    � AllowsTransparency .  � WindowStyle$
 None=�6[    � Icon$ Resources/Settings.png��6n    � WindowStartupLocation$ CenterScreen=�6�   ��5	      �� 5
   
    �� Color$ White��6    �� Opacity$ 0Q�6(   .��5      � $��4q�6    � UseLayoutRounding . 	 J� Effect	 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         
ShadowSize
   
BlurRadius#
 �   6     ShadowColor   Color# �  6I      ShadowDepth# �   6o   .+�5   
   �� $��2q�6     	MainColor#���  6%   ._�5      � -   5          BackgroundColor#���  5      .+�5      �� -   5         ��Name$
 Exit��5      $��29��6'     TranspButton#���  62   $��0,3,3,0q�6Y   $	��Top=�6j   $��24��6�   $��Right=�6�   .��5      �� ._�5      F�   Back#��[ 6   #���  6<    �� Stretch$ Uniform=�6_   5      5      5      �� 5      -   5         ��Name$ label��5      $�� ��6'   $
��Left=�62   $��10,3,0,0q�6M   $	��Top=�6_   $��Segoe UI Black)�6w   $��16(�6�     OnLightFontColor#���  6�   �� 5      -   5        $ Create��5        Editid1#��[ 6)   $��Right=�6L   $��	0,0,10,10q�6h   $��Bottom=�6{   $��22��6�   $��76��6�   �� 5      $	��219��6   $��
10,60,10,0q�6,   $	��Top=�6@   .��5      �� -   5         ��Name$ sw��5   !   5      �� 5      -   5        $ label1��5      #���  6(   $
��Left=�6L   $��
145,34,0,0q�6g   $	��Top=�6{   #���  6�   $
��Bold"�6�   $��67��6�   �� 5      -   5        $ label2��5       
 WDCid1#���  6(   $
��Left=�6K   $��
236,34,0,0q�6f   $	��Top=�6z   #���  6�   $
��Bold"�6�   �� 5       -   5        $ label3��5        
	 WDCid2#��� 	 6(   $
��Left=�6K   $��
364,34,0,0q�6f   $	��Top=�6z   #���  6�   $
��Bold"�6�   �� 5!      -	   5        $ label4��5!       

 WDCid3#��� 
 6(   $
��Left=�6K   $��
455,34,0,0q�6f   $	��Top=�6z   #���  6�   $
��Bold"�6�   5"      5#      5$      5%      