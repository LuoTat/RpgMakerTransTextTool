# RpgMakerTransTextTool

一款基于RPG-Maker VXAce Translator 和 AiNiee-chatgpt 的RPG游戏文本翻译工具<br>

使用方法：<br>
1.下载软件：RPG-Maker VXAce Translator version 0.10c<br>
  https://bitbucket.org/ArzorX/monster-girl-quest-paradox-translation/downloads/<br>
  此工具是用来提取RPG游戏的所有文本以及导入修改后的文本<br>
  使用教程：<br>
  https://arzorxblog.home.blog/2019/08/11/how-to-manual-patch-the-game-with-the-recent-translations/<br>
  （没错，我写这个小工具的初衷就是用来机翻MGQ的，只是随便借此机会练习使用GitHub）<br>

2.下载软件：AiNiee-chatgpt
  https://github.com/NEKOparapa/AiNiee-chatgpt<br>

3.用RPG-Maker VXAce Translator version 0.10c 提取所有的Scripts之后，再用此工具导出几乎所有的字符串为ManualTransFile.json文件，然后使用AiNiee-chatgpt来进行翻译，再用此工具把翻译好了的ManualTransFile.json里的字符串注入到原来的文本里面，之后会在软件的根目录里面生成一个Scrpts的文件夹，再把此文件夹里面的东西全部覆盖原来的Scripts就可以了<br>
