// src/store/loadingSlice.ts
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../store';

interface RowLoadingState {
  [entity: string]: Record<string, boolean>;
}

interface CellLoadingState {
  [entity: string]: Record<string, Record<string, boolean>>;
}

interface LoadingState {
  rows: RowLoadingState;
  cells: CellLoadingState;
}

const initialState: LoadingState = {
  cells: {},
  rows: {}
};

const loadingSlice = createSlice({
  initialState,
  name: 'loading',
  reducers: {
    setCellLoading: (state, action: PayloadAction<{ entity: string; id: string; field: string; isLoading: boolean }>) => {
      const { entity, id, field, isLoading } = action.payload;
      if (!state.cells[entity]) {
        state.cells[entity] = {};
      }
      if (!state.cells[entity][id]) {
        state.cells[entity][id] = {};
      }
      state.cells[entity][id][field] = isLoading;
    },
    setRowLoading: (state, action: PayloadAction<{ entity: string; id: string; isLoading: boolean }>) => {
      const { entity, id, isLoading } = action.payload;
      if (!state.rows[entity]) {
        state.rows[entity] = {};
      }
      state.rows[entity][id] = isLoading;
    }
  }
});

export const { setRowLoading, setCellLoading } = loadingSlice.actions;
export default loadingSlice.reducer;

export const selectIsRowLoading = (state: RootState, entity: string, id: string) => state.loading.rows[entity]?.[id] ?? false;
export const selectIsCellLoading = (state: RootState, entity: string, id: string, field: string) => state.loading.cells[entity]?.[id]?.[field] ?? false;
