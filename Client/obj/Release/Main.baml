   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient      Client.Main   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Client     -     � Title$  Client��$	��524��$	��735�� � 
ResizeMode$ NoResize=� � AllowsTransparency .  � WindowStyle$
 None=� � Icon$% Resources/wind-grasp-acid-3.png��.���� $��2q�   	MainColor#���   ._�� -     BackgroundColor#���  .+��� -    ��Name$ 	BSettings��$��40��  TranspButton#���  $��0,0,43,0q�$	��Top=�$��40��$��Right=�.���� ._�F�   Settings#��[ #���    �� Stretch$ Uniform=��� -   $ BLogout��$��40��#���  $��0,0,83,0q�$	��Top=�$��40��$��Right=�.���� ._�F�  
 Logout#��[ #���   $ Uniform=��� -   $ BExit��$��35��#���  $��0,3,8,0q�$	��Top=�$��35��$��Right=�.���� ._�F�   Exit#��[ #���   $ Uniform=��� -   $ button��  Mainid1#���  $
��Left=�$��	10,10,0,0q�$	��Top=�$��99��$��28��&   System.Windows.Controls.DataGrid  -     Name$ DGProp��$��10,69,10,12q�   AutoGenerateColumns . $y�Hidden=�	 � DataContext	 �� �� -   $ BUpdate��$��35��#���  $��	0,8,574,0q�$	��Top=�$��35��$��Right=�.���� ._�F�  
 Update#��[ #���   $ Uniform=��� -	   
 ��Name$
 checkBox��  Mainid2#���  $
��Left=�$��
188,17,0,0q�$	��Top=�$��21��$��94��� -
    �Name$ lmgrid��$��10,52,531,10q�.+�R� -    R�Name$ TreeObjects��$��0,26,0,0q�����   Client.Model   Models �� Source# Z ����  �� ItemsSource ��  �� Path$ Objects�.R��� ���� $
 Name� �� ItemTemplate �� .R��� ���� $
 Name��� -    ��Name$ label3�� 	 Editid4#��� 	 $��	24,0,22,0q�$	��Top=�$��Center=�$��Center=� 
 OnLightFontColor#��� 
 �� -    ��Name$ stackscroll��$��Right=�$��0,120,10,12q�$	��516��.���� -    ��Name$ stack��$	��516���� -   $ label��$
��Left=�$��
205,69,0,0q�$	��Top=�$	��516��$��Center=�$��Center=�$��Segoe UI Black)�$��20(�#��� 
 ���� *SelectedItem.Name+ �� ElementName$ TreeObjects���� -   $ label1��$��Right=�$��
0,10,133,0q�$	��Top=�$��Impact)�#���   $��14(�