   M S B A M L   `   `   `  ����F  BIOPT-Client, Version=1.0.1.2, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.WDashboard   5   	   U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	     � 
ResizeMode$  NoResize=�5       � AllowsTransparency .  � WindowStyle$
 None=�6J    � WindowStartupLocation$ CenterScreen=�6]    � Topmost .  � Icon$% Resources/wind-grasp-acid-3.png��6�    � WindowState$ Normal=�6�   $	��300��6�   $	��600��6�    � 	Resources 5         RectBorderStyle �� 
TargetType	 �� Property
 �� Value  	MainColor �� RelativeSource �� AncestorType��%�   &	        �� 5	   
   " ��6(   .�5
      �� 	 � |�
 .  6*   �� 5      	 � ��#
 �  6%   �� 5      	 �@�Tag
 6$   ��  �� $ WindowL�-   5        5      5      .��5      � .+�5   
   �� -   5         ��Name$ leftSizeGrip��5      $��7��6*   $
��Left=�64    � Cursor$ SizeWE��6O   #��[  6_   �� 5   
   -   5        $ rightSizeGrip��5      $��7��6+   $��Right=�65   $ SizeWE��6Q   #��[  6a   �� 5   
   -   5        $ topSizeGrip��5      $��7��6)   $	��Top=�64   $ SizeNS��6L   #��[  6\   �� 5   
   -   5        $ bottomSizeGrip��5      $��7��6,   $��Bottom=�67   $ SizeNS��6R   #��[  6b   �� 5   
   -   5        $��topLeftSizeGrip��5      $��7��6+   $��7��65   $
��Left=�6@   $	��Top=�6[   $ SizeNWSE��6s   #��[  6�   �� 5   
   -   5        $��bottomRightSizeGrip��5      $��7��6/   $��7��69   $��Right=�6D   $��Bottom=�6`   $ SizeNWSE��6{   #��[  6�   �� 5   
   -   5        $��topRightSizeGrip��5      $��7��6,   $��7��66   $��Right=�6A   $	��Top=�6]   $ SizeNESW��6u   #��[  6�   �� 5   
   -	   5        $��bottomLeftSizeGrip��5      $��7��6.   $��7��68   $
��Left=�6C   $��Bottom=�6^   $ SizeNESW��6y   #��[  6�   � 5   
   -
   5         �Name$ GMain��5        BackgroundColor#���  6   $��4q�6^   .+�5      �� -   5         ��Name$ BExit��5      $��35��6$     TranspButton#���  6/   $	��Top=�6V   $��35��6n   $��Right=�6z   $��5q�6�   .��5      �� ._�5      F�   Exit#��[ 6   #���  68    �� Stretch$ Uniform=�6[   5       5!      �� 5"      -   5        $ 	BMaximize��5"      $��35��6(   #���  63   $	��Top=�6Z   $��35��6r   $��Right=�6~   $��5,5,45,5q�6�   .��5#      �� ._�5$      F�   Maximize#��[ 6   #���  6<   $ Uniform=�6_   5%      5&      �� 5'      -   5        $ 	BMinimize��5'      $��35��6(   #���  63   $	��Top=�6Z   $��15��6r   $��Right=�6~   $��	5,15,85,5q�6�   .��5(      �� ._�5)      F�   Minimize#��[ 6   #���  6<   $ Uniform=�6_   5*      5+      �� 5,      -   5         ��Name$ Title��5,      $��Center=�6'   $
��Wrap=�6D   $	��Top=�6X   $��42��6p   $	��218��6|     OnLightFontColor#���  6�     FontFamilyHighlight#���  6�    	 FontSizeBig#��� 	 6�   $��0,20,0,0q�6  �� 5-      $��10,67,10,10q�6   .��5.      �� -   5         ��Name$ ContentPane��5.      5/      50      51      52      