# RpgMakerTransTextTool

一款基于RPG-Maker VXAce Translator 和 AiNiee-chatgpt 的RPG游戏文本翻译工具

使用方法：
1.下载软件：VX-Ace-Translator
  https://github.com/AhmedAhmedEG/VX-Ace-Translator
  此工具是用来提取RPG游戏的所有文本以及导入修改后的文本
  VX-Ace-Translator使用教程：
  详见GitHub上的说明

2.下载软件：AiNiee-chatgpt
  https://github.com/NEKOparapa/AiNiee-chatgpt

3.用VX-Ace-Translator 得到解包后的文件以及文本文件后，再用此工具导出其所有的字符串为ManualTransFile.json文件，然后使用AiNiee-chatgpt来进行翻译，再用此工具把翻译好了的ManualTransFile.json里的字符串注入到原来的文本里面，之后会在软件的根目录里面生成一个Scrpts的文件夹，再把此文件夹里面的东西全部覆盖到原来的解包文件夹里就可以了
