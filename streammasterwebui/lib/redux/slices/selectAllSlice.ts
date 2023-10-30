import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { type RootState } from '../store';

interface SetSelectAllPayload {
  isSelectAll: boolean
  typename: string
}

type SelectAllState = Record<string, boolean>

const initialState: SelectAllState = {};

const selectAllSlice = createSlice({
  initialState,
  name: 'selectAll',
  reducers: {
    setSelectAllInternal: (
      state,
      action: PayloadAction<SetSelectAllPayload>
    ) => {
      const { typename, isSelectAll } = action.payload;

      state[typename] = isSelectAll;
    }
  }
});

export const selectAll = (state: RootState, typename: string) => state.selectAll[typename];
export const { setSelectAllInternal } = selectAllSlice.actions;
export default selectAllSlice.reducer;
