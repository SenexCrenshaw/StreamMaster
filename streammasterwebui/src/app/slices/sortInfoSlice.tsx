import { type PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';
import { type RootState } from '../store';

type SetSortInfoPayload = {
  sortField?: string;
  sortOrder?: -1 | 0 | 1;
  typename: string;
}

type SortInfoState = Record<string, { orderBy: string, sortField: string, sortOrder: -1 | 0 | 1 }>;

const defaultSortInfo = {
  orderBy: 'id asc',
  sortField: 'id',
  sortOrder: 1 as -1 | 0 | 1
};
const initialState: SortInfoState = {};

const sortInfoSlice = createSlice({
  initialState,
  name: 'sortInfo',
  reducers: {
    setSortInfoInternal: (state, action: PayloadAction<SetSortInfoPayload>) => {
      const { typename, sortField, sortOrder } = action.payload;

      if (!state[typename]) {
        state[typename] = { ...defaultSortInfo };
      }

      if (sortField !== undefined) {
        state[typename].sortField = sortField;
      }

      if (sortOrder !== undefined) {
        state[typename].sortOrder = sortOrder;
      }

      if (state[typename].sortField && state[typename].sortOrder) {
        state[typename].orderBy = state[typename].sortField
          ? state[typename].sortOrder === -1
            ? `${state[typename].sortField} desc`
            : `${state[typename].sortField} asc`
          : '';

      }
    }
  }
});

export const sortInfo = (state: RootState, typename: string) => state.sortInfo[typename];
export const { setSortInfoInternal } = sortInfoSlice.actions;
export default sortInfoSlice.reducer;
