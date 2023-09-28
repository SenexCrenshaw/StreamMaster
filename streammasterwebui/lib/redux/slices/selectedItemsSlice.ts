
import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { type RootState } from '../../../lib/redux/store';

type SetSelectedItemsPayload = {
  items: any | undefined,
  typename: string,
};

type QuerySelectedItemsState = Record<string, any | undefined>;

const initialState: QuerySelectedItemsState = {};

const selectedItemsSlice = createSlice({
  initialState,
  name: 'selectedItems',
  reducers: {
    setSelectedItemsInternal: (state, action: PayloadAction<SetSelectedItemsPayload>) => {
      const { typename, items } = action.payload;

      if (!state[typename]) {
        state[typename] = [];
      }

      if (items !== null && items !== undefined) {
        state[typename] = items;
      } else {
        
        delete state[typename]; // Remove the key if the filter is null or undefined
      }
    },
  },
});

export const selectSelectedItems = (state: RootState, typename: string) => state.selectedItems[typename];
export const { setSelectedItemsInternal } = selectedItemsSlice.actions;
export default selectedItemsSlice.reducer;
