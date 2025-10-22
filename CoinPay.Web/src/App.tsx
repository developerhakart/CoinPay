import { useState } from 'react'

function App() {
  const [count, setCount] = useState(0)

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
      <div className="bg-white rounded-lg shadow-2xl p-8 max-w-md w-full">
        <h1 className="text-4xl font-bold text-gray-800 mb-4 text-center">
          CoinPay
        </h1>
        <p className="text-gray-600 text-center mb-6">
          Welcome to your cryptocurrency payment platform
        </p>

        <div className="bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg p-6 mb-6">
          <p className="text-white text-center text-xl mb-4">Counter: {count}</p>
          <button
            onClick={() => setCount((count) => count + 1)}
            className="w-full bg-white text-purple-600 font-semibold py-2 px-4 rounded-lg hover:bg-gray-100 transition duration-200"
          >
            Increment
          </button>
        </div>

        <div className="grid grid-cols-3 gap-4">
          <div className="bg-blue-50 rounded-lg p-4 text-center">
            <div className="text-2xl mb-2">💰</div>
            <p className="text-sm text-gray-600">Payments</p>
          </div>
          <div className="bg-purple-50 rounded-lg p-4 text-center">
            <div className="text-2xl mb-2">📊</div>
            <p className="text-sm text-gray-600">Analytics</p>
          </div>
          <div className="bg-indigo-50 rounded-lg p-4 text-center">
            <div className="text-2xl mb-2">🔒</div>
            <p className="text-sm text-gray-600">Security</p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default App
