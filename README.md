## Deskripsi
Simple Inventory Management System adalah aplikasi berbasis .NET Core MVC yang digunakan untuk mengelola stok barang dan transaksi terkait dalam suatu organisasi atau bisnis. Sistem ini dirancang untuk membantu pengguna melacak barang yang masuk dan keluar, serta memantau stock barang yang tersedia dalam inventaris. 

## Fitur
1. Authentication (Login & Logout)
2. CRUD (Create, Read, Update, Delete)
3. Role-based access
4. Item image upload
5. Generate pdf DinkToPdf

## Installasi

Clone repository
```bash
git clone https://github.com/dwipurnomo12/simple_inventory_management_system
```
Masuk ke folder project
```bash
cd nama-folder-proyek
```
Restore dependensi
```bash
dotnet restore
```
Konfigurasi database
```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb);Initial Catalog=db_inventory;Integrated Security=True;Pooling=False;Encrypt=False;Trust Server Certificate=True"
```
Migrasi database
```bash
dotnet ef database update
```
Menjalankan aplikasi
```bash
dotnet run
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)

## Screenshoot
![Screenshot_814](https://github.com/user-attachments/assets/249a647f-2311-4974-a989-a07c74751f6d)
![Screenshot_813](https://github.com/user-attachments/assets/0300b485-811f-40ae-bef6-f59981deee49)
![Screenshot_812](https://github.com/user-attachments/assets/44bf50d4-9831-43da-bad4-2adb03ebbbcb)
![Screenshot_811](https://github.com/user-attachments/assets/d53a80f7-899f-4b01-8578-85c5e2c016b4)
