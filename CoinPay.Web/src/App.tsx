import { RouterProvider } from 'react-router-dom';
import { ErrorBoundary } from '@/components/common/ErrorBoundary';
import { router } from '@/routes/router';
import { validateEnv } from '@/config';

// Validate environment configuration on app start
validateEnv();

function App() {
  return (
    <ErrorBoundary>
      <RouterProvider router={router} />
    </ErrorBoundary>
  );
}

export default App;
