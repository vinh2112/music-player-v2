   M S B A M L   `   `   `  ����J  FMusic Player v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Music_Player_v2Music_Player_v2  5   	       Music_Player_v2.Minimize   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:Music_Player_v2     5   	   b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$  Music Player��5      $	��200��6/   $	��300��6<    � ShowInTaskbar$ False��6H    � 
ResizeMode$ NoResize=�6^    � WindowStartupLocation$ Manual=�6t    � WindowStyle$
 None=�6�    � AllowsTransparency$
 True��6�   ��6�   Z� .��5
      � -   5        $��	TransGrid��5
      ���	#01FFFFFF � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $
��1.7* �6   �� 5      $��2* �6   �� 5      $��3* �6   .+�5   
   � $��0��6   $��Center=�6   .+�5      ]� .+�5      �� -   5        $��sliderProgress��5      $
��15 0q�6/    � Maximum$	 100Q�6=   ���#1DB954 �� IsMoveToPointEnabled$
 True��6a   6�   5      5      � 5   
   $��1��6   $	��Top=�6   .+�5      �� .+�5      �� -   5        $��NameSong��5      $	��200��6,   $��	Song Name��68   ���	#DDFFFFFF$��SemiBold"�6`   $��18(�6v   	 �� TextAlignment$	 Center=�6�   $��NoWrap=�6�   $��CharacterEllipsis=�6�   �� 5      -   5        $��
NameArtist��5      $	��200��6.   $��Artist Name��6:   ���	#DDFFFFFF$��Normal"�6d   $��14(�6x   $	 Center=�6�   $��NoWrap=�6�   $��CharacterEllipsis=�6�   5      5      � 5   
   $��2��6   $��Center=�6   .+�5       � $��Center=�6   .+�5!      ��  &  "MaterialDesignIconForegroundButton#��[  6   $��30��6U   $��30��6`   ���	#DDFFFFFF$��	10 0 10 0q�5"   q   ${�False��6�   ��6-   Z� ��6C   Z� 5#      �� 5$      -   5        $��PreviousSong��5$       , (MaterialDesignFloatingActionAccentButton#��[ 6-   $��30��6o   $��30��6z   ���	#DDFFFFFF$��	10 0 10 0q�5%   q   ��6-   Z� ��6C   Z� .��5&      '  !MaterialDesignThemes.Wpf.PackIcon  
   Kind$
 SkipPrevious=�6.   $��20��6C   $��20��6N   5'      �� 5(      -   5         ��Name$ btnPlay��5(      #��[ 6*   $��30��6l   $��30��6w   ���	#DDFFFFFF���	#DDFFFFFF$��	10 0 10 0q�5)   r   ��6-   Z� .��5*        $

 Play=�6.   $��20��6:   $��20��6E   5+      �� 5,      -   5        $ btnPause��5,      #��[ 6+   $��30��6m   $��30��6x   ���	#DDFFFFFF���	#DDFFFFFF$��	10 0 10 0q�5-   r   $y�	Collapsed=�6�   ��6-   Z� .��5.        $
 Pause=�6.   $��20��6;   $��20��6F   5/      �� 50      -   5        $��NextSong��50      #��[ 6)   $��30��6k   $��30��6v   ���	#DDFFFFFF$��	10 0 10 0q�51   q   ��6-   Z� ��6C   Z� .��52        $
 SkipNext=�6.   $��20��6>   $��20��6I   53      �� 54      -	   5        $
��Exit��54      #��[ 6%   $��30��6g   $��30��6r   ���	#DDFFFFFF$��	10 0 10 0q�55   q   ��6-   Z� ��6C   Z� .��56        $
 	ExitToApp=�6.   $��20��6@   $��20��6K   57      58      59      � 5;   
   $��3��6   .+�5<      �� $��50��6   $	��120��6$   ���	#01FFFFFF$
��Left=�6H   $��25 -41 10 10q�6c    K� ZIndex$	 100��6y   .+�5=      �� -
   5        $��Volume��5=        MaterialDesignIconButton#��[ 6'   $��30��6Y   $��30��6d   ���	#DDFFFFFF$��	10 0 10 0q�5>   q   ��6-   Z� ��6C   Z� .��5?        -   5        $��
VolumeIcon��5?   .   $
 
VolumeHigh=�6@   $��18��6S   $��18��6^   5@      �� 5A      -   5         ��Name$ SliderVolume��5A   $   $��0��6    � Value$	 0.1Q�6:   $ 1Q�6F   ���#1DB954$
 True��6h    �� Orientation$ Vertical=�6�   $��Center=�6�   $��Center=�6�    � LayoutTransform 5B      �� 5C       �� ScaleY$ -1Q�6)   5E       � Triggers 5F      :� 5G       :� RoutedEvent$ StackPanel.MouseEnter��6#   .[�5H      �� .��5I      �� -   5         ��Name$ ShowVolumePanel��5I   )   .��5J   "   W�  �� 
TargetName$ SliderVolume��62    �� TargetProperty$ Height�6W    g� 	BeginTime	 0:0:05K   2    W� From 06D   	 W� To 706M    g� Duration$ 0:0:0.2D�6U   5M   #   5N      5O      5P      :� 5R      $ StackPanel.MouseLeave��6#   .[�5S      �� .��5T      �� -   5        $ HideVolumePanel��5T   )   .��5U   "   W� $ SliderVolume��62   $ Height�6W   	 0:0:05V   2    706D    06N   $ 0:0:0.2D�6U   5X   #   5Y      5Z      5[      5]      5^      5_      5`      