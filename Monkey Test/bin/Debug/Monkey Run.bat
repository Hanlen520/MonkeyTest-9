@echo off

::---------�����ļ�-------------
::LOG��־���·��
set FILE_PATH=.\
if not exist %FILE_PATH%\Log ( md %FILE_PATH%\Log )
set LOG=%FILE_PATH%\Log\Log.txt
::------------------------------

echo.��ʼ���� && echo....
adb shell monkey -p com.uuabc.samakenglish --pct-touch 40 --pct-motion 10 --pct-syskeys 0 --pct-trackball 0 --ignore-security-exceptions --ignore-timeouts --ignore-crashes --monitor-native-crashes --throttle 0 -v-v-v 0 >%LOG%
echo.���Խ��� && echo.

findstr "ANR" "%LOG%" > nul && if not exist %FILE_PATH%\Error ( md %FILE_PATH%\Error ) && echo.����ANR����ȡLog��...&& adb pull data/anr/traces.txt > %FILE_PATH%\Error\traces.txt && adb shell logcat -d -v time > %FILE_PATH%\Error\Log_ANR.txt && adb bugreport > %FILE_PATH%\Error\BugReport.txt && echo ------��ȡ��ɣ�
findstr "CRASH" "%LOG%" > nul &&  if not exist %FILE_PATH%\Error ( md %FILE_PATH%\Error ) && echo.����CRASH����ȡLog��...&& adb shell logcat -d -v time > %FILE_PATH%\Error\Log_Crash.txt && adb bugreport > %FILE_PATH%\Error\BugReport.txt && echo ------��ȡ��ɣ�

echo.
pause
