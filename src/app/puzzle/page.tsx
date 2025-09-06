import Link from "next/link";

export default function PuzzlePage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-green-400 via-blue-500 to-purple-600">
      {/* Header */}
      <header className="bg-white/90 backdrop-blur-sm shadow-lg">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <Link href="/" className="flex items-center space-x-3 hover:opacity-80">
              <div className="w-12 h-12 bg-gradient-to-br from-yellow-400 to-orange-500 rounded-xl flex items-center justify-center">
                <span className="text-2xl">🧩</span>
              </div>
              <h1 className="text-2xl font-bold text-gray-800">Puzzle Mode</h1>
            </Link>
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

      {/* Puzzle Game Demo */}
      <section className="container mx-auto px-4 py-12">
        <div className="text-center mb-12">
          <h2 className="text-4xl font-bold text-white mb-4">
            🧩 Mode Puzzle Interaktif
          </h2>
          <p className="text-xl text-white/90 mb-8">
            Demonstrasi gameplay puzzle Unity dengan drag & drop mechanics
          </p>
        </div>

        {/* Game UI Mockup */}
        <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl max-w-4xl mx-auto mb-12">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center space-x-4">
              <div className="bg-blue-100 px-4 py-2 rounded-full">
                <span className="text-blue-800 font-bold">Level 5</span>
              </div>
              <div className="bg-green-100 px-4 py-2 rounded-full">
                <span className="text-green-800 font-bold">⏱️ 02:35</span>
              </div>
            </div>
            <div className="flex items-center space-x-2">
              <button className="bg-yellow-500 hover:bg-yellow-600 text-white px-4 py-2 rounded-full font-bold">
                💡 Hint (2)
              </button>
              <button className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-full font-bold">
                ⏸️ Pause
              </button>
            </div>
          </div>

          {/* Progress Bar */}
          <div className="mb-8">
            <div className="flex items-center justify-between mb-2">
              <span className="text-gray-700 font-semibold">Progress:</span>
              <span className="text-gray-700 font-semibold">3/5 pieces</span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-4">
              <div className="bg-green-500 h-4 rounded-full w-3/5"></div>
            </div>
          </div>

          {/* Puzzle Area */}
          <div className="grid grid-cols-2 gap-8">
            {/* Puzzle Pieces */}
            <div className="bg-gray-50 rounded-xl p-6">
              <h3 className="text-lg font-bold text-gray-800 mb-4">🔧 Puzzle Pieces</h3>
              <div className="grid grid-cols-3 gap-4">
                <div className="w-16 h-16 bg-red-400 rounded-lg cursor-pointer hover:scale-110 transition-transform flex items-center justify-center text-white font-bold">
                  ●
                </div>
                <div className="w-16 h-16 bg-blue-400 rounded-lg cursor-pointer hover:scale-110 transition-transform flex items-center justify-center text-white font-bold">
                  ■
                </div>
                <div className="w-16 h-16 bg-green-400 rounded-lg cursor-pointer hover:scale-110 transition-transform flex items-center justify-center text-white font-bold">
                  ▲
                </div>
                <div className="w-16 h-16 bg-yellow-400 rounded-lg cursor-pointer hover:scale-110 transition-transform flex items-center justify-center text-white font-bold">
                  ★
                </div>
                <div className="w-16 h-16 bg-purple-400 rounded-lg cursor-pointer hover:scale-110 transition-transform flex items-center justify-center text-white font-bold">
                  ♥
                </div>
                <div className="w-16 h-16 bg-pink-400 rounded-lg cursor-pointer hover:scale-110 transition-transform flex items-center justify-center text-white font-bold">
                  ◆
                </div>
              </div>
            </div>

            {/* Drop Targets */}
            <div className="bg-gray-50 rounded-xl p-6">
              <h3 className="text-lg font-bold text-gray-800 mb-4">🎯 Drop Targets</h3>
              <div className="grid grid-cols-3 gap-4">
                <div className="w-16 h-16 bg-red-200 border-2 border-dashed border-red-400 rounded-lg flex items-center justify-center text-red-600 font-bold">
                  ●
                </div>
                <div className="w-16 h-16 bg-blue-200 border-2 border-dashed border-blue-400 rounded-lg flex items-center justify-center">
                  <div className="w-8 h-8 bg-blue-400 rounded"></div>
                </div>
                <div className="w-16 h-16 bg-green-200 border-2 border-dashed border-green-400 rounded-lg flex items-center justify-center">
                  <div className="w-8 h-8 bg-green-400 transform rotate-45"></div>
                </div>
                <div className="w-16 h-16 bg-yellow-200 border-2 border-dashed border-yellow-400 rounded-lg flex items-center justify-center text-yellow-600 font-bold">
                  ?
                </div>
                <div className="w-16 h-16 bg-purple-200 border-2 border-dashed border-purple-400 rounded-lg flex items-center justify-center text-purple-600 font-bold">
                  ?
                </div>
                <div className="w-16 h-16 bg-pink-200 border-2 border-dashed border-pink-400 rounded-lg flex items-center justify-center text-pink-600 font-bold">
                  ?
                </div>
              </div>
            </div>
          </div>

          {/* Success Animation Area */}
          <div className="mt-8 text-center">
            <div className="inline-flex items-center space-x-2 bg-green-100 px-6 py-3 rounded-full">
              <span className="text-green-600 text-2xl animate-bounce">🎉</span>
              <span className="text-green-800 font-bold">Great job! Keep going!</span>
              <span className="text-green-600 text-2xl animate-bounce">✨</span>
            </div>
          </div>
        </div>

        {/* Unity Features */}
        <div className="grid md:grid-cols-3 gap-8 mb-12">
          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-4">🎮 Unity Scripts</h3>
            <ul className="space-y-2 text-gray-700">
              <li>• <strong>PuzzleManager.cs</strong> - Core logic</li>
              <li>• <strong>PuzzlePiece.cs</strong> - Individual pieces</li>
              <li>• <strong>PlayerController.cs</strong> - Touch input</li>
              <li>• <strong>DropTarget.cs</strong> - Validation</li>
            </ul>
          </div>

          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-4">🎯 Gameplay Features</h3>
            <ul className="space-y-2 text-gray-700">
              <li>• Drag & drop mechanics</li>
              <li>• Visual feedback animations</li>
              <li>• Progressive difficulty</li>
              <li>• Hint system dengan limit</li>
            </ul>
          </div>

          <div className="bg-white/90 backdrop-blur-sm rounded-xl p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-4">📱 Mobile Optimized</h3>
            <ul className="space-y-2 text-gray-700">
              <li>• Large touch targets (80px+)</li>
              <li>• Haptic feedback support</li>
              <li>• Auto-save progress</li>
              <li>• Orientation support</li>
            </ul>
          </div>
        </div>

        {/* Code Preview */}
        <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-8 shadow-xl mb-12">
          <h3 className="text-2xl font-bold text-gray-800 mb-6">💻 Unity C# Code Preview</h3>
          <div className="bg-gray-900 rounded-lg p-6 font-mono text-sm overflow-x-auto">
            <div className="text-green-400">// PuzzleManager.cs - Core puzzle logic</div>
            <div className="text-blue-300 mt-2">public class PuzzleManager : MonoBehaviour</div>
            <div className="text-white ml-4 mt-1">{'{'}</div>
            <div className="text-yellow-300 ml-8">public void OnPieceDropped(PuzzlePiece piece, DropTarget target)</div>
            <div className="text-white ml-8">{'{'}</div>
            <div className="text-gray-300 ml-12">if (ValidatePlacement(piece, target))</div>
            <div className="text-white ml-12">{'{'}</div>
            <div className="text-gray-300 ml-16">HandleCorrectPlacement();</div>
            <div className="text-gray-300 ml-16">PlaySuccessAnimation();</div>
            <div className="text-gray-300 ml-16">AudioManager.Instance.PlayPuzzleComplete();</div>
            <div className="text-white ml-12">{'}'}</div>
            <div className="text-gray-300 ml-12">else HandleIncorrectPlacement();</div>
            <div className="text-white ml-8">{'}'}</div>
            <div className="text-white ml-4">{'}'}</div>
            <div className="text-green-400 mt-4">// Dilengkapi dengan komentar bahasa Indonesia</div>
            <div className="text-green-400">// untuk kemudahan pemahaman dan maintenance</div>
          </div>
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