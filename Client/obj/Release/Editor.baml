   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient      Client.Editor   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Client       � Title$  Editor��$	��457��$	��962�� � 
ResizeMode$ NoResize=� � AllowsTransparency .  � WindowStyle$
 None=� � Icon$% Resources/wind-grasp-acid-3.png��.���� $��2q�   	MainColor#���   ._�� -     BackgroundColor#���  $��0,0,-2,0q�.+��� -    ��Name$ BBack��$��40��  TranspButton#���  $��0,3,8,0q�$	��Top=�$��40��$��Right=�.���� ._�F�   Back#��[ #���    �� Stretch$ Uniform=�� -    �Name$ editOptions��$
��Left=�$	��266�����	#BFF7941D$��
-268,0,0,0q� K� ZIndex$ 40��.+��� -   	 ��Name$	 TBName��$��23��$��
32,77,33,0q�$�� ��$	��Top=��� -   
 ��Name$
 	labelName��  Editid2#���  $��
60,46,60,0q�$	��Top=�$��Center=�$��Center=�  OnLightFontColor#���  �� -   $
 labelScript��  Editid8#���  $��55,105,55,0q�$	��Top=�$��Center=�$��Center=�#���  $y�Hidden=��� -    ��Name$ CBType��$��33,131,32,0q�$	��Top=�$��1q�#���   � DataContext �� �� -   $
 	labelType��  Editid3#���  $��60,105,60,0q�$	��Top=�$��Center=�$��Center=�#���  �� -	   $ BSave��  Editid9#���  $��Stretch=�$��55,334,55,0q�$	��Top=��� -
   $
 
labelValue�� 	 Viewid4#��� 	 $��87,158,86,0q�$	��Top=�$��Center=�$��Center=��� -   $	 TBValue��$��23��$��33,189,32,0q�$�� ��$	��Top=�] YICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310'  !ICSharpCode.AvalonEdit.TextEditor  ?
avalonEdit.http://icsharpcode.net/sharpdevelop/avalonedit  -     Name$ TBScript��$��Consolas)�   SyntaxHighlightingM  GICSharpCode.AvalonEdit.Highlighting.HighlightingDefinitionTypeConverter$ 
JavaScript $
��10pt(�$��10,131,10,132q�� $��
10,52,0,10q�$
��Left=�$	��247��.+��� -    ��Name$ Lmodels��$��	0,26,0,25q� �� ���� .R��  � ColumnDefinitions �� $	��145 ��� $��20 ��� $��20 ��� $��20 �.+���  
 OnDarkFontColor#��� 
 $
��Left=�$��0������ *Name+�� -   #���  $��Right=�$��Center=�$��1��
 � Tag �� .���� ._�F�   Edit#��[ #��� 
 $ Uniform=��� -   #���  $��Right=�$��Center=�$��2�� �� .���� ._�F�   Copy#��[ #��� 
 $ Uniform=��� -   #���  $��Right=�$��Center=�$��3�� �� .���� ._�F�  
 Delete#��[ #��� 
 $ Uniform=��� -   $
 label3��  Editid4#���  $��	24,0,22,0q�$	��Top=�$��Center=�$��Center=�#���  �� -   $ button1��  Editid1#���  $��Stretch=�$��	13,0,12,0q�$��Bottom=�$��20��� $��0,52,458,10q�$��Right=�$	��240��.+��� -   $ Lobjects��$��	0,26,0,25q����� *SelectedItem.Objects+ �� ElementName$ Lmodels������ .R��  �� $	��144 ��� $��20 ��� $��20 ��� $��20 �.+��� #��� 
 $
��Left=�$��0������ *Name+�� -   #���  $��Right=�$��Center=�$��1�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -   #���  $��Right=�$��Center=�$��2�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -   #���  $��Right=�$��Center=�$��3�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -   $
 label4��  Editid5#���  $
��10,0q�$	��Top=�$��Center=�$��Center=�#���  �� -   $ button2��#���  $��Stretch=�$��	12,0,13,0q�$��Bottom=�$��20��� $��0,52,226,10q�$��Right=�$	��227��.+��� -   $ Lproperties��$��	0,26,0,25q����� *SelectedItem.Properties+$ Lobjects������ .R��  �� $	��133 ��� $��20 ��� $��20 ��� $��20 �.+��� #��� 
 $
��Left=�$��0������ *Name+�� -   #���  $��Right=�$��Center=�$��1�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -   #���  $��Right=�$��Center=�$��2�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -   #���  $��Right=�$��Center=�$��3�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -   $
 label5��  Editid6#���  $
��10,0q�$	��Top=�$��Center=�$��Center=�#���  �� -   $ button3��#���  $��Stretch=�$��	12,0,13,0q�$��Bottom=�$��20��� $��	0,52,8,10q�$��Right=�$	��213��.+��� -   $ Lscripts��$��	0,26,0,25q����� *SelectedItem.Scripts+$ Lproperties������ .R��  �� $	��120 ��� $��20 ��� $��20 ��� $��20 �.+��� #��� 
 $
��Left=�$��0������ *Name+�� -    #���  $��Right=�$��Center=�$��1�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -!   #���  $��Right=�$��Center=�$��2�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -"   #���  $��Right=�$��Center=�$��3�� �� .���� ._�F� #��[ #��� 
 $ Uniform=��� -#   $
 label6��  Editid7#���  $
��10,0q�$	��Top=�$��Center=�$��Center=�#���  �� -$   $ button4��#���  $��Stretch=�$��	12,0,13,0q�$��Bottom=�