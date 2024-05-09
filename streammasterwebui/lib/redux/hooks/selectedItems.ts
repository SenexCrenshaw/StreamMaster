import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectedItemsPayload {
  value: any | undefined;
  key: string;
}

type SelectedItemsState = Record<string, any | undefined>;

const initialState: SelectedItemsState = {};

const selectedItemsSlice = createSlice({
  initialState,
  name: 'selectedItems',
  reducers: {
    SetSelectedItems: (state, action: PayloadAction<SetSelectedItemsPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectedItems = createSelector(
  (state: RootState) => state.selectedItems,
  (selectedItems) => (key: string) => selectedItems[key]
);

const castToArrayOfType = <T>(data: any): T[] => {
  if (data === undefined) {
    return [] as T[];
  }

  if (Array.isArray(data)) {
    return data as T[];
  }

  throw new Error('Data is not of the expected type.', data);
};

export const useSelectedItems = <T>(key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedItems = (value: T[] | undefined) => {
    dispatch(
      selectedItemsSlice.actions.SetSelectedItems({
        key,
        value
      })
    );
  };

  const rawSelectedItems = useSelector((state: RootState) => selectSelectedItems(state)(key));
  const selectedItems = castToArrayOfType<T>(rawSelectedItems);

  return { setSelectedItems, selectedItems };
};

export default selectedItemsSlice.reducer;
