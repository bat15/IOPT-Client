   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   	MainColor  �� Color  BackgroundColor  ButtonOverColor  ButtonClickColor  DisabledColor  SelectionColor  OnLightFontColor  OnDarkFontColor �� 
TargetType �� Property �� Value  ButtonBaseControlTemplate3   -System.Windows.Media.Effects.DropShadowEffect    ShadowDepth    
BlurRadius    Color �� Triggers :� RoutedEvent	 �� TargetProperty
 W� From	 W� To g� Duration g� AutoReverse �� 
TargetType ��Name J� SnapsToDevicePixels ��Name �� ContentStringFormat �� ContentStringFormat �� Triggers P� Property P� Value �� 
TargetName �� Padding ]� 
Conditions �� Property �� Value �� 	Resources=  7System.Windows.Controls.Primitives.DataGridColumnHeader&   System.Windows.Controls.DataGrid*  $System.Windows.Controls.DataGridCell �� RelativeSource �� CornerRadius 	 ScrollThumbs �Name  � RowDefinitions! ]�Name" ]� Thumb# n�Name$ ]� IncreaseRepeatButton% ��Name& J� Opacity' ]� DecreaseRepeatButton( P� 
SourceName 
 ButtonSelectBrush  	DarkBrush) �� Angle  ComboBoxToggleButton* � ColumnDefinitions+ F�Name  ComboBoxTextBox, �� 	ClickMode- d� 	IsChecked. �� Path/ �� Mode0 J� IsHitTestVisible1 �� SelectionBoxItem2 �� SelectionBoxItemTemplate3 ��Name4 �� 
IsReadOnly5 �� 
IsReadOnly6 �� IsDropDownOpen7 � AllowsTransparency8 � ActualWidth9 �� MaxDropDownHeight: K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation;   DirectionalNavigation  TranspButton # TranspButtonBaseControlTemplate  CheckRadioFocusVisual< �� StrokeDashArray= � 	Resources  Key> �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame? O� KeyTime@ O� ValueA �� 
BackgroundB �� BulletC J� RenderTransformOriginD J� EffectE    	DirectionF N� EnterActionsG ��Name  StoryboardIsCheckedH �� BeginStoryboardName  StoryboardIsCheckedOff�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     localclr-namespace:Client  %�(  &	        &	      &	 $     &	 7     &	 J     &	 _     &	 r     &	 �     '�� �     7  7   7   &	      7  7  7  7  7   7   7  7  7  '�� �    7   7  7  '��     7   '      7   7  '  �    7  7   '  �	    '�� 
    7  7   7  '�� �
    7  7   7  7  7   7   7  7   7  &		 Q    '�� �    7   7  '� �    7  7   7  &	 g    7   7   7   7   &	 x    '�� �    7  7  7  7   7  7   7   '�� �    7  7   7   &	     &	 �    7  &	 x    '�� �    7  7   7  7  7  7  'R� �%    7  7   7  'Q� &&    7  �� $  #8bc34a���� $  White���� $  #aed581���� $  #33691e���� $  	#af8bc34a���� $  #F7941D���� $  Black���� $  White���� " �����  � ��$ Segoe UI)���  � ��$ 12(���  � ��$	 Top=���  � ��$
 Left=���  � ��# �  ��  � ��8   ��  � ��8  ��  � ��8  ��  �@J�SnapsToDevicePixels . ��  �@J�Effect    $ 3Q�$ 3Q�$ Black�� :� $ Button.Click��.[��� .���� .��W� $	 Effect.ShadowDepth�$
 3X�$ 0X�$ 0:0:0.2D� . �� " ��.R��� $ border��#��z��#��z��#��z�� . ._��� $ contentPresenter��#��z��#��z��# z |�.  #��z��#��z����. # z #��z�� P�  �@��IsDefaulted . . ��� $ border��# � �� � ��P�  �@J�IsMouseOver . . ��� $ border��8    � ���� $ border��8   � ����  �@�Cursor$
 Hand��P�  � �� . . ��� $ border��8   � ���� $ border��8   � ��P�  �@d�	IsChecked$
 TrueY�. ��� $ border��8   � ���� $ border��8   � ��P�  � {� .  . ��� $ border��8   � ���� $ border��8   � ���� $ contentPresenter��8   � ���� " ��.���  � ��8   ��  � ��8  ��  � ��$ 0q���  � ��$ 1q���  �@J�SnapsToDevicePixels . ��  � �� �� " ��.R��� $ Bd��#��z��#��z��#��z��$ 
10,10,0,10q� . ._��� #��z��# z #��z�� ]�  ��  �@��
IsSelected . ./��� $ Bd��8   � ����  � ��
 ������� " ��.���  � ��
 ���� ��  � ��8   ��  � ��$ 1q���  � ��$ Disabled=� ���� $  Transparent��(��       * SystemColors.HighlightBrushKey+)�� $  Transparent��(��       *SystemColors.ControlBrushKey+)�� "  .���  � ��$ Center=���  � ��$ 30����  � ��8   ��  � ��8  ��  � ��$ 0,0,10,0q���  �@J�SnapsToDevicePixels . �� "  .���  �@ CanUserAddRows .  ��  �@ CanUserDeleteRows .  ��  �@ GridLinesVisibility$
 None=���   �@ AlternatingRowBackground �	#7faed581��  �@ RowBackground8   ��  � ��
 �������  � ��8  ��  �@ 
IsReadOnly . ��  �@ 	CellStyle �� "  .���  � ��$ 0q���  � �� �� *
Foreground+ �� *TemplatedParent+��  � �� �� *
Background+ �� *TemplatedParent+�� "  .���  � ��$ 0,0,10,0q���  � �� �� "  .R��� # z��#��z��#��z��#��z�� . ._��� # z �� " �����  � ��8   ��  � ��8  ��  � ��8  ��  �@��TextWrapping$
 Wrap=���  � ��$ Segoe UI)���  � ��$ 12(���  �@J�SnapsToDevicePixels . �� " ��.���  � ��8   ��  � ��8  ��  � ��$ Impact)���  � ��$ 16(���  �@J�SnapsToDevicePixels . ��  � �� �� " ��.R�� .+��� $��Border��$��0,0,0,0q�
������ 
������ $��0,0,0,0q�$ 0��._��� $ ContentSite��$��Center=�$��Center=�$��Header��$��32,22,32,22q���.  P�  �@��
IsSelected . . ���  �@K�ZIndex$	 100���� $ Border��8   � ���� $ Border�� � ��$ 1,1,1,0q��� 8   � ���� 8   � ��P�  � {� .  . ��� $ Border��8   � ���� $ Border��8   � ���� 8   � ���� 8   � ���� " n�.���  � �� �� " n�.R�� $
 Grid��.+��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��
������ �� $ 
Rectangle1��$ 0��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��#��z�� P�  �@�Tag$ 
Horizontal��. ��� $ 
Rectangle1�� � ��$
 Auto���� $ 
Rectangle1�� � ��$ 7���� " ��.���  �@��IsFlicksEnabled .  ��  � ��8   ��  � ��8  ��  � ��$ 2����  � �� �� " ��.R�� $ GridRoot��$��8��#��z��  �� $��0.00001* �.+�]� $! 
PART_Track��$��0����. |�.  " n� $# Thumb��#��z��#��� 	 $ �� $% PageUp��$��ScrollBar.PageDownCommand��$& 0Q�|�.  ' �� $% PageDown��$��ScrollBar.PageUpCommand��$& 0Q�|�.   P� $( Thumb�� �@J�IsMouseOver . . ��� # � 
 $ Thumb�� � ��P� $( Thumb�� �@n�
IsDragging . . ��� # �  $ Thumb�� � ��P�  � {� .  . ��� $ Thumb�� � y�$ 	Collapsed=�P�  �@��Orientation$ 
Horizontal=�. ��� $ GridRoot�� �@�LayoutTransform �� $	) -90Q��� $ 
PART_Track�� �@�LayoutTransform �� $	) -90Q���  � ��$
 Auto����  � ��$ 5���� $ Thumb�� �@�Tag$ 
Horizontal���� $ PageDown�� � ��$ ScrollBar.PageLeftCommand���� $ PageUp�� � ��$  ScrollBar.PageRightCommand���� " ����  � ��8   ��  � ��8  ��  � ��8  ��  �@�IsIndeterminate . ��  �@J�SnapsToDevicePixels . ��  � y�$ Hidden=���  �@K�ZIndex$	 100���� " d�.R�� * �� �� $��20 �.+��� $ Border��$��2��$ 0��8��  8�� $��1q��� $��0��$ 0��$��1q�8�� 8�� $��0,0,1,0q�F� $+ Arrow��$��1��
�������$��Center=�$��Center=�=����        �    �� � 	= ��[ � z �� � z     � 	=  	= 	�� " ��.R��� $ PART_ContentHost��|�.  #��z���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  �@�Cursor$
 Hand����  � ��$	 120����  � ��$ 20����  � ��8   ��  � �� �� " ��.R�� .+�d� $��ToggleButton��8�� $��2��|�.  $, Press=�- �� $. IsDropDownOpen�$/ TwoWay=� �� *TemplatedParent+�� $��ContentSite��0 .  #��z1 #��z2 #��z��$��3,3,23,3q�$��Center=�$
��Left=��� $3 PART_EditableTextBox��8�� $
��Left=�$��Center=�$��3,3,23,3q�|�. 8�� 
��� � �$y�Hidden=�#5 z4 ��Z� � $��Popup��$��Bottom=�#��z6 7 . |�.  $��Slide=�.��� $��DropDown�� . #��z8 #��z9 .+��� $ DropDownBorder��8�� $��1q�8�� �� $��4,6,4,6q� . .���� : . $; 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P�  � {� .  . ��� 8   � ��P�  �@��
IsGrouping . . ���  � �� .  P� $( Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder�� �@��CornerRadius$ 0���� $ DropDownBorder�� � ��$ 0,2,0,0q�P�  �@��
IsEditable . . ���  � �� .  �� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=� �� " ��.���  �@J�SnapsToDevicePixels . ��  � ��8   ��  �@�OverridesDefaultStyle . ��  � �� �� " ��.R��� $��Border��$ 6q� . ._���  P�  �@��IsHighlighted . . ��� $ Border��8   � ��P�  � {� .  . ��� 8   � ���� " �����  � ��$ Segoe UI)���  � ��$ 12(���  � ��$	 Top=���  � ��$
 Left=���  � ��# �  ��  � ��
 ���� ��  � ��
 ���� ��  � ��
 ���� ��  �@J�SnapsToDevicePixels . ��  �@J�Effect    $ 3Q�$ 3Q�$ Black�� :� $ Button.Click��.[��� .���� .��W� $	 Effect.ShadowDepth�$
 3X�$ 0X�$ 0:0:0.2D� . �� " ��.R��� $ border��#��z��#��z��#��z�� . ._��� $ contentPresenter��#��z��#��z��# z |�.  #��z��#��z����. # z #��z�� P�  �@��IsDefaulted . . ��� $ border��# � �� � ��P�  �@J�IsMouseOver . . ��� $ border�� � ��
 ���� �� $ border�� � ��
 ���� ��  �@�Cursor$
 Hand��P�  � �� . . ��� $ border�� � ��
 ���� �� $ border�� � ��
 ���� P�  �@d�	IsChecked$
 TrueY�. ��� $ border�� � ��
 ���� �� $ border�� � ��
 ���� P�  � {� .  . ��� $ border�� � ��
 ���� �� $ border�� � ��
 ���� �� $ contentPresenter��8    � ���� ���  � �� �� .R��� $��14,0,0,0q� . #��� ��$��1��$	< 1 2R��� " ��.���  � ��$ 1q���  � ��8   ��  � ��8  ��  � ��8  ��  � ��$ Center=���  � ��$ Center=���  � ��$ Segoe UI)���  � ��$ 12(���  �@J�SnapsToDevicePixels . ��  �@�Cursor$
 Hand����  � �� �� " ��= ���� StoryboardIsChecked .��U� $U	 O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�$> 	CheckFlag��.\�  $? 0��$@ 0Q�  $? 0:0:0.2��$@ 14Q��� StoryboardIsCheckedOff .��U� $U	 O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�$> 	CheckFlag��.\�  $? 0��$@ 14Q�  $? 0:0:0.2��$@ 0Q�.R��� 
A ����  . B �� $ ForegroundPanel��$��1q�$��35��$��20��$ 10��._��� .+��� $ 	CheckFlag��
�������$ 10��$��Center=�$��1q�$��19��$��18��$C 0.5,0.5&�z�V� .~��� �� �� T� D    $ 1Q�$	E 180Q�._��� #��z��#��z����. # z $��Center=� P�  � �� . . ��� 8   �@�FocusVisualStyle��  � ��$ 4,0,0,0q�P�  � {� .  . ��� # � �� � ��P�  �@d�	IsChecked$
 TrueY�. ��� $ ForegroundPanel�� � �� �#8bc34aF �� $ G BeginStoryboardCheckedTrue��8�� �� $!H BeginStoryboardCheckedFalse��P�  �@d�	IsChecked$ FalseY�. ��� $ ForegroundPanel�� � �� �#f44336F �� $!G BeginStoryboardCheckedFalse��8�� �� $ H BeginStoryboardCheckedTrue���� " R����  � ��8   ��  � ��8  ��  � ��8  ��  � ��$ Segoe UI)���  � ��$ 12(���  �@J�SnapsToDevicePixels . �� " Q����  � ��8   ��  � ��
 ���� ��  � ��
 ���� ��  � ��$ Segoe UI)���  � ��$ 12(���  �@J�SnapsToDevicePixels . 