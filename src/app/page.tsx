import Image from "next/image";
import Link from "next/link";

export default function Home() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-400 via-purple-500 to-pink-500">
      {/* Header */}
      <header className="bg-white/90 backdrop-blur-sm shadow-lg">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <div className="w-12 h-12 bg-gradient-to-br from-yellow-400 to-orange-500 rounded-xl flex items-center justify-center">
                <span className="text-2xl">🧩</span>
              </div>
              <h1 className="text-2xl font-bold text-gray-800">Creative Puzzle Adventure</h1>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2 bg-yellow-100 px-4 py-2 rounded-full">
                <span className="text-yellow-600">⭐</span>
                <span className="font-semibold text-yellow-800">1,247</span>
              </div>
              <div className="flex items-center space-x-2 bg-blue-100 px-4 py-2 rounded-full">
                <span className="text-blue-600">💰</span>
                <span className="font-semibold text-blue-800">2,850</span>
              </div>
            </div>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <section className="container mx-auto px-4 py-12">
        <div className="text-center mb-12">
          <h2 className="text-5xl font-bold text-white mb-4">
            Game Puzzle Kreatif untuk Anak
          </h2>
          <p className="text-xl text-white/90 mb-8 max-w-3xl mx-auto">
            Sebuah game Unity 2D yang mengombinasikan puzzle logika dengan mode kreativitas. 
            Dirancang khusus untuk anak-anak dengan interface yang ramah dan gameplay yang edukatif.
          </p>
          <div className="flex flex-wrap justify-center gap-4">
            <Link 
              href="/puzzle" 
              className="bg-green-500 hover:bg-green-600 text-white font-bold text-xl px-8 py-4 rounded-full shadow-lg transform hover:scale-105 transition-all duration-200"
            >
              🎮 Main Puzzle
            </Link>
            <Link 
              href="/creativity" 
              className="bg-purple-500 hover:bg-purple-600 text-white font-bold text-xl px-8 py-4 rounded-full shadow-lg transform hover:scale-105 transition-all duration-200"
            >
              🎨 Mode Kreativitas
            </Link>
          </div>
        </div>

        {/* Game Features */}
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8 mb-16">
          <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl">
            <div className="text-center mb-6">
              <div className="w-16 h-16 bg-gradient-to-br from-red-400 to-pink-500 rounded-full flex items-center justify-center mx-auto mb-4">
                <span className="text-3xl">🧩</span>
              </div>
              <h3 className="text-2xl font-bold text-gray-800">Puzzle Logika</h3>
            </div>
            <ul className="space-y-3 text-gray-700">
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Mencocokkan bentuk dan warna</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Menyusun pola dan urutan</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Drag and drop yang mudah</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Level bertingkat kesulitan</li>
            </ul>
          </div>

          <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl">
            <div className="text-center mb-6">
              <div className="w-16 h-16 bg-gradient-to-br from-purple-400 to-blue-500 rounded-full flex items-center justify-center mx-auto mb-4">
                <span className="text-3xl">🎨</span>
              </div>
              <h3 className="text-2xl font-bold text-gray-800">Mode Kreativitas</h3>
            </div>
            <ul className="space-y-3 text-gray-700">
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Menggambar bebas dengan kuas</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Palette warna cerah</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Bentuk dan stiker</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Simpan karya seni</li>
            </ul>
          </div>

          <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl">
            <div className="text-center mb-6">
              <div className="w-16 h-16 bg-gradient-to-br from-yellow-400 to-orange-500 rounded-full flex items-center justify-center mx-auto mb-4">
                <span className="text-3xl">🏆</span>
              </div>
              <h3 className="text-2xl font-bold text-gray-800">Sistem Reward</h3>
            </div>
            <ul className="space-y-3 text-gray-700">
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Bintang dan koin</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Koleksi stiker lucu</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Kostum karakter</li>
              <li className="flex items-center"><span className="text-green-500 mr-2">✅</span> Achievement system</li>
            </ul>
          </div>
        </div>

        {/* Unity Project Structure */}
        <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl mb-12">
          <h3 className="text-3xl font-bold text-gray-800 mb-6 text-center">Struktur Project Unity</h3>
          <div className="grid md:grid-cols-2 gap-8">
            <div>
              <h4 className="text-xl font-bold text-gray-700 mb-4">📁 Assets Structure</h4>
              <div className="bg-gray-50 p-4 rounded-lg font-mono text-sm">
                <div>Assets/</div>
                <div className="ml-4">├── Scripts/</div>
                <div className="ml-8">│   ├── Core/ (GameManager, AudioManager)</div>
                <div className="ml-8">│   ├── Player/ (PlayerController)</div>
                <div className="ml-8">│   ├── Puzzle/ (PuzzleManager, PuzzlePiece)</div>
                <div className="ml-8">│   ├── UI/ (UIManager)</div>
                <div className="ml-8">│   ├── Rewards/ (RewardSystem)</div>
                <div className="ml-8">│   └── Creativity/ (DrawingManager)</div>
                <div className="ml-4">├── Scenes/</div>
                <div className="ml-4">├── Prefabs/</div>
                <div className="ml-4">└── Audio/</div>
              </div>
            </div>
            <div>
              <h4 className="text-xl font-bold text-gray-700 mb-4">⚙️ Key Features</h4>
              <ul className="space-y-2">
                <li><strong>Cross-Platform:</strong> Android, iOS, WebGL</li>
                <li><strong>Child-Friendly UI:</strong> Tombol besar, warna cerah</li>
                <li><strong>Touch Optimized:</strong> Drag & drop untuk mobile</li>
                <li><strong>Audio System:</strong> Musik dan SFX yang menyenangkan</li>
                <li><strong>Progress Tracking:</strong> Save system otomatis</li>
                <li><strong>Modular Design:</strong> Mudah diperluas</li>
              </ul>
            </div>
          </div>
        </div>

        {/* Technical Specs */}
        <div className="grid md:grid-cols-3 gap-6 mb-12">
          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6 text-center">
            <h4 className="text-lg font-bold text-gray-800 mb-2">🎮 Platform</h4>
            <p className="text-gray-600">Unity 2D - Mobile & Web</p>
          </div>
          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6 text-center">
            <h4 className="text-lg font-bold text-gray-800 mb-2">📱 Target</h4>
            <p className="text-gray-600">Anak-anak usia 4-12 tahun</p>
          </div>
          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6 text-center">
            <h4 className="text-lg font-bold text-gray-800 mb-2">💻 Bahasa</h4>
            <p className="text-gray-600">C# dengan komentar lengkap</p>
          </div>
        </div>

        {/* Screenshots Preview */}
        <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl">
          <h3 className="text-3xl font-bold text-gray-800 mb-8 text-center">Preview Game Screens</h3>
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div className="bg-gradient-to-br from-blue-100 to-blue-200 rounded-xl p-6 text-center">
              <img 
                src="https://storage.googleapis.com/workspace-0f70711f-8b4e-4d94-86f1-2a93ccde5887/image/b188ceb2-8a4b-4294-b7b0-2a8e4d363bba.png" 
                alt="Main Menu dengan tombol besar dan colorful" 
                className="w-full h-32 object-cover rounded-lg mb-4"
              />
              <h4 className="font-bold text-gray-800">Main Menu</h4>
              <p className="text-sm text-gray-600">Interface ramah anak dengan tombol besar</p>
            </div>
            
            <div className="bg-gradient-to-br from-green-100 to-green-200 rounded-xl p-6 text-center">
              <img 
                src="https://storage.googleapis.com/workspace-0f70711f-8b4e-4d94-86f1-2a93ccde5887/image/8473eac7-1eb0-4e57-b6f4-63de40a3270e.png" 
                alt="Puzzle gameplay dengan pieces berwarna" 
                className="w-full h-32 object-cover rounded-lg mb-4"
              />
              <h4 className="font-bold text-gray-800">Puzzle Game</h4>
              <p className="text-sm text-gray-600">Drag & drop puzzle pieces yang menarik</p>
            </div>
            
            <div className="bg-gradient-to-br from-purple-100 to-purple-200 rounded-xl p-6 text-center">
              <img 
                src="https://storage.googleapis.com/workspace-0f70711f-8b4e-4d94-86f1-2a93ccde5887/image/2184e951-a993-4ffd-aad6-1b79db07e269.png" 
                alt="Drawing canvas dengan tools warna-warni" 
                className="w-full h-32 object-cover rounded-lg mb-4"
              />
              <h4 className="font-bold text-gray-800">Mode Kreativitas</h4>
              <p className="text-sm text-gray-600">Canvas menggambar dengan tools lengkap</p>
            </div>
            
            <div className="bg-gradient-to-br from-yellow-100 to-yellow-200 rounded-xl p-6 text-center">
              <img 
                src="https://storage.googleapis.com/workspace-0f70711f-8b4e-4d94-86f1-2a93ccde5887/image/9f9d78b1-9a63-4a78-a965-fd20f60e4c78.png" 
                alt="Reward screen dengan bintang dan koin" 
                className="w-full h-32 object-cover rounded-lg mb-4"
              />
              <h4 className="font-bold text-gray-800">Reward System</h4>
              <p className="text-sm text-gray-600">Celebrasi dengan bintang dan koin</p>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-white/90 backdrop-blur-sm mt-16">
        <div className="container mx-auto px-4 py-8 text-center">
          <p className="text-gray-600 mb-4">
            Unity 2D Creative Puzzle Adventure - Game edukatif untuk anak-anak
          </p>
          <div className="flex justify-center space-x-6">
            <div className="text-sm text-gray-500">✅ Cross-Platform Compatible</div>
            <div className="text-sm text-gray-500">✅ Child-Friendly Design</div>
            <div className="text-sm text-gray-500">✅ Educational Gameplay</div>
          </div>
        </div>
      </footer>
    </div>
  );
}