# RestaurantMenu

WPF-приложение для управления меню ресторанов, кафе и кофеен.  
Данные хранятся в файлах **JSON** и **XML** в папке `Files`.

## Требования

- Windows
- .NET Framework 4.7.2
- Visual Studio 2019 или новее (с поддержкой WPF)

## Структура решения

```
RestaurantMenu/
├── Model/                    — библиотека классов
│   ├── Core/                 — Dish, Menu, Establishment, Restaurant, Cafe, CoffeeShop
│   └── Data/                 — JsonDataService, XmlDataService, FilePaths
├── RestaurantMenu/           — WPF-приложение
│   ├── MainWindow            — главное окно
│   ├── MenuWindow            — просмотр и редактирование меню
│   ├── AddEstablishmentWindow — добавление нового заведения
│   └── Images/               — PNG-иконки (котики)
└── Files/                    — данные заведений (копируются в bin при сборке)
```

## Запуск

1. Откройте `RestaurantMenu.slnx` в Visual Studio.
2. Восстановите NuGet-пакеты (для проекта `Model`).
3. Установите **RestaurantMenu** как стартовый проект.
4. Нажмите **F5** (Debug) или **Ctrl+F5** (без отладки).

Файлы данных ищутся в папке `bin\Debug\Files\` рядом с `.exe`.

## Возможности

- Загрузка заведений из JSON/XML при старте
- Фильтр по типу: все / рестораны / кафе / кофейни
- Просмотр обычного и сезонного меню
- Добавление, изменение и удаление блюд
- Сохранение в JSON или XML, конвертация между форматами
- Добавление нового заведения через кнопку **«➕ Новое заведение»**

## Изображения (Images)

Положите PNG с прозрачным фоном в папку `Images/`:

| Файл | Назначение |
|------|------------|
| `cat_logo_left.png` | Главное окно, слева |
| `cat_logo_rigth.png` | Главное окно, справа |
| `cat_menu.png` | Окно меню |
| `cat_add.png` | Окно нового заведения |

В свойствах каждого файла в Visual Studio: **Build Action → Resource**.

## Формат данных (JSON)

```json
{
  "Name": "Название",
  "Address": "Адрес",
  "Type": "Restaurant",
  "CurrentMenu": {
    "Dishes": [
      { "Name": "Блюдо", "Price": 100, "Category": "Hot" }
    ]
  },
  "IsSeasonal": false,
  "SeasonalDishes": []
}
```

Типы заведений: `Restaurant`, `Cafe`, `CoffeeShop`.  
Категории блюд: `Hot`, `Drink`, `Dessert`, `Cold`.

## Сборка из командной строки

```powershell
msbuild RestaurantMenu.slnx /p:Configuration=Debug
```

---

*Учебный проект — управление меню заведений общепита.*
