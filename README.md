# Приложение для интеграции систем Ecwid и Frontpad.

Приложение проверяет новые заказы в Ecwid и отсылает их в Frontpad, общается с этими системама по API.
Запускается один раз и каждый 15 минут осуществляет проверку на наличие новых заказов в Ecwid. В файле **appInfo.json** хранится id последнего отправленного заказа (каждый раз перезаписывает). Приложение отправляет все заказы, чьи id больше указанного в файле. Файл сам копируется в сборку. 

**Перед первым запуском НУЖНО:**
1. Установить в файле **appInfo.json** id нужного заказа. При первой проверке приложение отправит все заказы с id больше указанного, далее сам будет перезаписывать этот файл. (заказ с id из файла передан не будет!)
2. Заполнить поля в файле **appSettings.json**. (и обязательно удалять, если вдруг пушите какие-то изменения!)

Логи хранятся в папке logs.

