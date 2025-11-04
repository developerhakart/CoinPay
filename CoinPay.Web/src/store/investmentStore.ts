import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import {
  ExchangeConnectionStatus,
  InvestmentPlan,
  InvestmentPosition,
  InvestmentPositionDetail
} from '@/types/investment';

/**
 * Investment Store - Sprint N04: Phase 4 Exchange Investment
 * Manages state for exchange connections, plans, and investment positions
 */

interface InvestmentState {
  // Exchange Connection
  connectionStatus: ExchangeConnectionStatus | null;
  isConnected: boolean;

  // Investment Plans
  plans: InvestmentPlan[];
  selectedPlan: InvestmentPlan | null;

  // Investment Positions
  positions: InvestmentPosition[];
  selectedPosition: InvestmentPositionDetail | null;
  totalPortfolioValue: number;
  totalPrincipal: number;
  totalRewards: number;

  // UI State
  isLoading: boolean;
  isConnecting: boolean;
  isCreatingInvestment: boolean;
  error: string | null;

  // Modals/Wizards
  showConnectionModal: boolean;
  showCreateWizard: boolean;
  showPositionDetails: boolean;
}

interface InvestmentActions {
  // Connection Actions
  setConnectionStatus: (status: ExchangeConnectionStatus) => void;
  setConnecting: (isConnecting: boolean) => void;
  toggleConnectionModal: () => void;

  // Plans Actions
  setPlans: (plans: InvestmentPlan[]) => void;
  selectPlan: (plan: InvestmentPlan | null) => void;

  // Positions Actions
  setPositions: (positions: InvestmentPosition[]) => void;
  addPosition: (position: InvestmentPosition) => void;
  updatePosition: (id: string, updates: Partial<InvestmentPosition>) => void;
  setSelectedPosition: (position: InvestmentPositionDetail | null) => void;
  calculatePortfolioTotals: () => void;

  // UI Actions
  setLoading: (isLoading: boolean) => void;
  setCreatingInvestment: (isCreating: boolean) => void;
  setError: (error: string | null) => void;
  clearError: () => void;
  toggleCreateWizard: () => void;
  togglePositionDetails: () => void;

  // Reset
  reset: () => void;
}

type InvestmentStore = InvestmentState & InvestmentActions;

const initialState: InvestmentState = {
  // Exchange Connection
  connectionStatus: null,
  isConnected: false,

  // Investment Plans
  plans: [],
  selectedPlan: null,

  // Investment Positions
  positions: [],
  selectedPosition: null,
  totalPortfolioValue: 0,
  totalPrincipal: 0,
  totalRewards: 0,

  // UI State
  isLoading: false,
  isConnecting: false,
  isCreatingInvestment: false,
  error: null,

  // Modals/Wizards
  showConnectionModal: false,
  showCreateWizard: false,
  showPositionDetails: false,
};

export const useInvestmentStore = create<InvestmentStore>()(
  devtools(
    (set, get) => ({
      ...initialState,

      // ========================================================================
      // CONNECTION ACTIONS
      // ========================================================================

      setConnectionStatus: (status) =>
        set(
          {
            connectionStatus: status,
            isConnected: status.connected,
            isConnecting: false,
          },
          false,
          'investment/setConnectionStatus'
        ),

      setConnecting: (isConnecting) =>
        set({ isConnecting }, false, 'investment/setConnecting'),

      toggleConnectionModal: () =>
        set(
          (state) => ({ showConnectionModal: !state.showConnectionModal }),
          false,
          'investment/toggleConnectionModal'
        ),

      // ========================================================================
      // PLANS ACTIONS
      // ========================================================================

      setPlans: (plans) =>
        set({ plans }, false, 'investment/setPlans'),

      selectPlan: (plan) =>
        set({ selectedPlan: plan }, false, 'investment/selectPlan'),

      // ========================================================================
      // POSITIONS ACTIONS
      // ========================================================================

      setPositions: (positions) => {
        set({ positions }, false, 'investment/setPositions');
        get().calculatePortfolioTotals();
      },

      addPosition: (position) => {
        set(
          (state) => ({
            positions: [position, ...state.positions],
          }),
          false,
          'investment/addPosition'
        );
        get().calculatePortfolioTotals();
      },

      updatePosition: (id, updates) => {
        set(
          (state) => ({
            positions: state.positions.map((pos) =>
              pos.id === id ? { ...pos, ...updates } : pos
            ),
          }),
          false,
          'investment/updatePosition'
        );
        get().calculatePortfolioTotals();
      },

      setSelectedPosition: (position) =>
        set(
          { selectedPosition: position },
          false,
          'investment/setSelectedPosition'
        ),

      calculatePortfolioTotals: () => {
        const { positions } = get();

        const totalPortfolioValue = positions.reduce(
          (sum, pos) => sum + pos.currentValue,
          0
        );

        const totalPrincipal = positions.reduce(
          (sum, pos) => sum + pos.principalAmount,
          0
        );

        const totalRewards = positions.reduce(
          (sum, pos) => sum + pos.accruedRewards,
          0
        );

        set(
          { totalPortfolioValue, totalPrincipal, totalRewards },
          false,
          'investment/calculatePortfolioTotals'
        );
      },

      // ========================================================================
      // UI ACTIONS
      // ========================================================================

      setLoading: (isLoading) =>
        set({ isLoading }, false, 'investment/setLoading'),

      setCreatingInvestment: (isCreatingInvestment) =>
        set(
          { isCreatingInvestment },
          false,
          'investment/setCreatingInvestment'
        ),

      setError: (error) =>
        set({ error }, false, 'investment/setError'),

      clearError: () =>
        set({ error: null }, false, 'investment/clearError'),

      toggleCreateWizard: () =>
        set(
          (state) => ({ showCreateWizard: !state.showCreateWizard }),
          false,
          'investment/toggleCreateWizard'
        ),

      togglePositionDetails: () =>
        set(
          (state) => ({ showPositionDetails: !state.showPositionDetails }),
          false,
          'investment/togglePositionDetails'
        ),

      // ========================================================================
      // RESET
      // ========================================================================

      reset: () =>
        set(initialState, false, 'investment/reset'),
    }),
    { name: 'InvestmentStore' }
  )
);

export default useInvestmentStore;
