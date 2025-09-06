import Link from "next/link";

export default function CreativityPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-purple-400 via-pink-500 to-red-500">
      {/* Header */}
      <header className="bg-white/90 backdrop-blur-sm shadow-lg">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <Link href="/" className="flex items-center space-x-3 hover:opacity-80">
              <div className="w-12 h-12 bg-gradient-to-br from-purple-400 to-pink-500 rounded-xl flex items-center justify-center">
                <span className="text-2xl">🎨</span>
              </div>
              <h1 className="text-2xl font-bold text-gray-800">Creativity Mode</h1>
            </Link>
            <div className="flex items-center space-x-4">
              <button className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-full font-bold">
                💾 Save Art
              </button>
              <button className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-full font-bold">
                🗑️ Clear
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Drawing Canvas Demo */}
      <section className="container mx-auto px-4 py-8">
        <div className="text-center mb-8">
          <h2 className="text-4xl font-bold text-white mb-4">
            🎨 Mode Kreativitas Digital
          </h2>
          <p className="text-xl text-white/90 mb-6">
            Demonstrasi drawing canvas Unity dengan tools yang ramah anak
          </p>
        </div>

        {/* Main Canvas Area */}
        <div className="bg-white/95 backdrop-blur-sm rounded-2xl shadow-xl overflow-hidden max-w-6xl mx-auto">
          {/* Drawing Tools */}
          <div className="bg-gray-100 p-4 border-b">
            <div className="flex flex-wrap items-center justify-between gap-4">
              {/* Drawing Tools */}
              <div className="flex items-center space-x-2">
                <span className="text-gray-700 font-semibold">Tools:</span>
                <button className="bg-blue-500 text-white px-4 py-2 rounded-full font-bold hover:bg-blue-600 transition-colors">
                  🖌️ Brush
                </button>
                <button className="bg-gray-300 text-gray-700 px-4 py-2 rounded-full font-bold hover:bg-gray-400 transition-colors">
                  🧽 Eraser
                </button>
                <button className="bg-gray-300 text-gray-700 px-4 py-2 rounded-full font-bold hover:bg-gray-400 transition-colors">
                  ⭕ Circle
                </button>
                <button className="bg-gray-300 text-gray-700 px-4 py-2 rounded-full font-bold hover:bg-gray-400 transition-colors">
                  ⭐ Star
                </button>
              </div>

              {/* Brush Size */}
              <div className="flex items-center space-x-2">
                <span className="text-gray-700 font-semibold">Size:</span>
                <input 
                  type="range" 
                  min={5} 
                  max={50} 
                  defaultValue={15}
                  className="w-24"
                />
                <span className="text-gray-700 font-mono">15px</span>
              </div>
            </div>
          </div>

          {/* Color Palette */}
          <div className="bg-gray-50 p-4 border-b">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-2">
                <span className="text-gray-700 font-semibold">Colors:</span>
                <div className="flex space-x-2">
                  <div className="w-10 h-10 bg-red-500 rounded-full cursor-pointer hover:scale-110 transition-transform border-4 border-red-600"></div>
                  <div className="w-10 h-10 bg-blue-500 rounded-full cursor-pointer hover:scale-110 transition-transform"></div>
                  <div className="w-10 h-10 bg-green-500 rounded-full cursor-pointer hover:scale-110 transition-transform"></div>
                  <div className="w-10 h-10 bg-yellow-500 rounded-full cursor-pointer hover:scale-110 transition-transform"></div>
                  <div className="w-10 h-10 bg-purple-500 rounded-full cursor-pointer hover:scale-110 transition-transform"></div>
                  <div className="w-10 h-10 bg-pink-500 rounded-full cursor-pointer hover:scale-110 transition-transform"></div>
                  <div className="w-10 h-10 bg-orange-500 rounded-full cursor-pointer hover:scale-110 transition-transform"></div>
                  <div className="w-10 h-10 bg-cyan-500 rounded-full cursor-pointer hover:scale-110 transition-transform"></div>
                </div>
              </div>

              <div className="flex items-center space-x-4">
                <button className="bg-purple-400 hover:bg-purple-500 text-white px-4 py-2 rounded-full font-bold">
                  🎨 Color Mixer
                </button>
                <button className="bg-pink-400 hover:bg-pink-500 text-white px-4 py-2 rounded-full font-bold">
                  🖼️ Stamps
                </button>
              </div>
            </div>
          </div>

          {/* Canvas Area */}
          <div className="p-8">
            <div className="bg-white border-4 border-dashed border-gray-300 rounded-xl min-h-96 flex items-center justify-center relative overflow-hidden">
              {/* Sample Drawing */}
              <div className="absolute inset-4">
                {/* Sample brush strokes */}
                <div className="absolute top-8 left-8">
                  <div className="w-32 h-8 bg-red-400 rounded-full opacity-80"></div>
                </div>
                <div className="absolute top-16 left-12">
                  <div className="w-24 h-6 bg-blue-400 rounded-full opacity-80"></div>
                </div>
                
                {/* Sample shapes */}
                <div className="absolute top-12 right-16">
                  <div className="w-16 h-16 bg-yellow-400 rounded-full opacity-80"></div>
                </div>
                <div className="absolute bottom-16 left-16">
                  <div className="w-12 h-12 bg-green-400 transform rotate-45 opacity-80"></div>
                </div>
                <div className="absolute bottom-12 right-12">
                  <div className="w-14 h-14 bg-purple-400 opacity-80" style={{clipPath: 'polygon(50% 0%, 0% 100%, 100% 100%)'}}>
                  </div>
                </div>

                {/* Center placeholder */}
                <div className="absolute inset-0 flex items-center justify-center">
                  <div className="text-center text-gray-400">
                    <div className="text-6xl mb-4">🎨</div>
                    <div className="text-xl font-semibold">Drawing Canvas</div>
                    <div className="text-sm">Touch & drag to draw!</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="bg-gray-100 p-4 border-t">
            <div className="flex justify-between items-center">
              <div className="flex space-x-2">
                <button className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-full font-bold">
                  ↶ Undo
                </button>
                <button className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-full font-bold">
                  ↷ Redo
                </button>
              </div>
              <div className="flex space-x-2">
                <button className="bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded-full font-bold">
                  📷 Save Drawing
                </button>
                <button className="bg-orange-500 hover:bg-orange-600 text-white px-6 py-2 rounded-full font-bold">
                  📤 Share
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Unity Features */}
        <div className="grid md:grid-cols-3 gap-8 mt-12 mb-12">
          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-4">🎨 Drawing System</h3>
            <ul className="space-y-2 text-gray-700">
              <li>• <strong>DrawingManager.cs</strong> - Canvas control</li>
              <li>• <strong>ColorPicker.cs</strong> - Color selection</li>
              <li>• LineRenderer untuk smooth strokes</li>
              <li>• Touch-optimized input handling</li>
            </ul>
          </div>

          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-4">🛠️ Creative Tools</h3>
            <ul className="space-y-2 text-gray-700">
              <li>• Brush dengan berbagai ukuran</li>
              <li>• Shape tools (circle, star, heart)</li>
              <li>• Color mixing untuk belajar warna</li>
              <li>• Stamp dan sticker collection</li>
            </ul>
          </div>

          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-4">💾 Save & Share</h3>
            <ul className="space-y-2 text-gray-700">
              <li>• Screenshot capture system</li>
              <li>• Local storage untuk artwork</li>
              <li>• Gallery untuk melihat karya</li>
              <li>• Export ke device gallery</li>
            </ul>
          </div>
        </div>

        {/* Color Mixing Feature */}
        <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl mb-12">
          <h3 className="text-2xl font-bold text-gray-800 mb-6 text-center">🌈 Color Mixing Feature</h3>
          <div className="grid md:grid-cols-2 gap-8">
            <div>
              <h4 className="text-lg font-bold text-gray-700 mb-4">Educational Color Learning</h4>
              <p className="text-gray-600 mb-4">
                Anak-anak bisa belajar tentang warna dengan fitur color mixing yang interaktif. 
                Mereka dapat menggabungkan warna dasar untuk menciptakan warna baru.
              </p>
              <div className="flex items-center space-x-4">
                <div className="flex items-center space-x-2">
                  <div className="w-8 h-8 bg-red-500 rounded-full"></div>
                  <span>+</span>
                  <div className="w-8 h-8 bg-blue-500 rounded-full"></div>
                  <span>=</span>
                  <div className="w-8 h-8 bg-purple-500 rounded-full"></div>
                </div>
              </div>
            </div>
            <div>
              <h4 className="text-lg font-bold text-gray-700 mb-4">Unity Implementation</h4>
              <div className="bg-gray-900 rounded-lg p-4 font-mono text-sm">
                <div className="text-green-400">// ColorPicker.cs</div>
                <div className="text-blue-300">public Color MixColors(Color color1, Color color2)</div>
                <div className="text-white">{'{'}</div>
                <div className="text-gray-300 ml-4">return Color.Lerp(color1, color2, 0.5f);</div>
                <div className="text-white">{'}'}</div>
              </div>
            </div>
          </div>
        </div>

        {/* Sticker Collection */}
        <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl mb-12">
          <h3 className="text-2xl font-bold text-gray-800 mb-6 text-center">🏷️ Sticker & Stamp Collection</h3>
          <div className="grid grid-cols-6 md:grid-cols-8 lg:grid-cols-12 gap-4 mb-6">
            <div className="aspect-square bg-yellow-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">⭐</div>
            <div className="aspect-square bg-pink-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">💖</div>
            <div className="aspect-square bg-blue-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🌟</div>
            <div className="aspect-square bg-green-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🍀</div>
            <div className="aspect-square bg-purple-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🦄</div>
            <div className="aspect-square bg-orange-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🌈</div>
            <div className="aspect-square bg-red-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🎈</div>
            <div className="aspect-square bg-cyan-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🎂</div>
            <div className="aspect-square bg-indigo-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🎪</div>
            <div className="aspect-square bg-teal-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🎭</div>
            <div className="aspect-square bg-lime-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🎨</div>
            <div className="aspect-square bg-amber-100 rounded-lg flex items-center justify-center text-2xl cursor-pointer hover:scale-110 transition-transform">🎵</div>
          </div>
          <p className="text-center text-gray-600">
            Koleksi stiker yang dapat dibeli dengan koin yang dikumpulkan dari puzzle
          </p>
        </div>

        {/* Back Navigation */}
        <div className="text-center">
          <Link 
            href="/"
            className="bg-white/90 hover:bg-white text-gray-800 font-bold text-lg px-8 py-4 rounded-full shadow-lg transform hover:scale-105 transition-all duration-200 inline-block"
          >
            ← Kembali ke Home
          </Link>
        </div>
      </section>
    </div>
  );
}