using System;
using System.Collections.Generic;

class Sekolah
{
    public string idSekolah { get; set; }
    public string namaSekolah { get; set; }
    public int jumlahPaketDiterima { get; set; }

    public Sekolah(string id, string nama, int diterima = 0)
    {
        idSekolah = id;
        namaSekolah = nama;
        jumlahPaketDiterima = diterima;
    }

    public void TampilInfo()
    {
        Console.WriteLine($"[Sekolah] ID: {idSekolah} | Nama: {namaSekolah} | Diterima: {jumlahPaketDiterima} paket");
    }
}

abstract class PaketMBG
{
    private string idPaket;
    private string namaPaket;
    private int stokDapur;

    public string IdPaket { get => idPaket; set => idPaket = value; }
    public string NamaPaket { get => namaPaket; set => namaPaket = value; }
    public int StokDapur { get => stokDapur; }

    public PaketMBG(string id, string nama, int stok)
    {
        idPaket = id;
        namaPaket = nama;
        stokDapur = (stok < 0) ? 0 : stok;
    }

    public bool KurangiStok(int jumlah)
    {
        if (jumlah <= 0 || jumlah > stokDapur)
        {
            return false;
        }
        stokDapur -= jumlah;
        return true;
    }

    public abstract double HitungBiaya(int jumlah);
}

class PaketReguler : PaketMBG
{
    public PaketReguler(string id, string nama, int stok) : base(id, nama, stok) { }

    public override double HitungBiaya(int jumlah)
    {
        return jumlah * 15000;
    }
}

class PaketKhusus : PaketMBG
{
    public string jenisDiet { get; set; }

    public PaketKhusus(string id, string nama, int stok, string diet) : base(id, nama, stok)
    {
        jenisDiet = diet;
    }

    public override double HitungBiaya(int jumlah)
    {
        return jumlah * 20000;
    }
}

interface DapatDidistribusikan
{
    void KirimPaket();
}

class DistribusiMBG : DapatDidistribusikan
{
    public Sekolah sekolahTujuan { get; set; }
    public PaketMBG paketMakanan { get; set; }
    public int jumlah { get; set; }

    public DistribusiMBG(Sekolah sekolah, PaketMBG paket, int jumlah)
    {
        sekolahTujuan = sekolah;
        paketMakanan = paket;
        this.jumlah = jumlah; 
    }

    public void KirimPaket()
    {
        if (jumlah <= 0)
        {
            Console.WriteLine("Distribusi tidak bisa");
            return;
        }

        if (paketMakanan.KurangiStok(jumlah))
        {
            sekolahTujuan.jumlahPaketDiterima += jumlah;
            Console.WriteLine($"Berhasil: {jumlah} paket dikirim ke {sekolahTujuan.namaSekolah}");
        }
        else
        {
            Console.WriteLine($"Stok dapur tidak cukup {paketMakanan.NamaPaket}");
        }
    }
}

class PengelolaMBG
{
    private List<Sekolah> daftarSekolah = new List<Sekolah>();
    private List<PaketMBG> daftarPaket = new List<PaketMBG>();

    public void TambahSekolah(Sekolah sekolah) => daftarSekolah.Add(sekolah);
    public void TambahPaket(PaketMBG paket) => daftarPaket.Add(paket);

    public void TampilkanDataMBG()
    {
        Console.WriteLine("\nData Sekolah Penerima");
        foreach (var s in daftarSekolah)
        {
            s.TampilInfo();
        }

        Console.WriteLine("\n Data Paket Dapur");
        foreach (var p in daftarPaket)
        {
            Console.WriteLine($"[Paket] ID: {p.IdPaket} | Nama: {p.NamaPaket} | Sisa Stok: {p.StokDapur}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
      
        Sekolah sdn01 = new Sekolah("SCH-01", "SMPN 1 PUGER");
        Sekolah smp02 = new Sekolah("SCH-02", "SMPN 02 JEMBER");
        
        PaketReguler paketAyam = new PaketReguler("PKT-01", "Nasi Goreng", 100);
        PaketKhusus paketVegetarian = new PaketKhusus("PKT-02", "Nasi Soto", 50, "Vegetarian");

        PengelolaMBG pengelola = new PengelolaMBG();
        pengelola.TambahSekolah(sdn01);
        pengelola.TambahSekolah(smp02);
        pengelola.TambahPaket(paketAyam);
        pengelola.TambahPaket(paketVegetarian);

        Console.WriteLine("STATUS AWAL");
        pengelola.TampilkanDataMBG();

        int pesananAyam = 30;
        Console.WriteLine($"\nEstimasi Biaya {pesananAyam} porsi {paketAyam.NamaPaket}: Rp{paketAyam.HitungBiaya(pesananAyam):N0}");

        Console.WriteLine("\nPENGIRIMAN");
        DistribusiMBG distribusi1 = new DistribusiMBG(sdn01, paketAyam, 30);
        distribusi1.KirimPaket();

        DistribusiMBG distribusi2 = new DistribusiMBG(smp02, paketVegetarian, 60); 
        distribusi2.KirimPaket();

        Console.WriteLine("\nSTATUS AKHIR");
        pengelola.TampilkanDataMBG();
    }
}