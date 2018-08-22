@echo off

::---------配置文件-------------
::LOG日志存放路径
set FILE_PATH=.\
if not exist %FILE_PATH%\Log ( md %FILE_PATH%\Log )
set LOG=%FILE_PATH%\Log\Log.txt
::------------------------------

echo.开始测试 && echo....
adb shell monkey -p com.uuabc.samakenglish --pct-touch 40 --pct-motion 10 --pct-syskeys 0 --pct-trackball 0 --ignore-security-exceptions --ignore-timeouts --ignore-crashes --monitor-native-crashes --throttle 0 -v-v-v 0 >%LOG%
echo.测试结束 && echo.

findstr "ANR" "%LOG%" > nul && if not exist %FILE_PATH%\Error ( md %FILE_PATH%\Error ) && echo.发现ANR，提取Log中...&& adb pull data/anr/traces.txt > %FILE_PATH%\Error\traces.txt && adb shell logcat -d -v time > %FILE_PATH%\Error\Log_ANR.txt && adb bugreport > %FILE_PATH%\Error\BugReport.txt && echo ------提取完成！
findstr "CRASH" "%LOG%" > nul &&  if not exist %FILE_PATH%\Error ( md %FILE_PATH%\Error ) && echo.发现CRASH，提取Log中...&& adb shell logcat -d -v time > %FILE_PATH%\Error\Log_Crash.txt && adb bugreport > %FILE_PATH%\Error\BugReport.txt && echo ------提取完成！

echo.
pause
