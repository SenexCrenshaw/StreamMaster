import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectedPostalCodePayload {
  value: string | null;
  key: string;
}

type SelectedPostalCodeState = Record<string, string | null>;

const initialState: SelectedPostalCodeState = {};

const selectedPostalCodeSlice = createSlice({
  initialState,
  name: 'selectedPostalCode',
  reducers: {
    SetSelectedPostalCode: (state, action: PayloadAction<SetSelectedPostalCodePayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectedPostalCode = createSelector(
  (state: RootState) => state.selectedPostalCode,
  (selectedPostalCode) => (key: string) => selectedPostalCode[key]
);

export const useSelectedPostalCode = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedPostalCode = (value: string | null) => {
    dispatch(
      selectedPostalCodeSlice.actions.SetSelectedPostalCode({
        key,
        value
      })
    );
  };

  const selectedPostalCode = useSelector((state: RootState) => selectSelectedPostalCode(state)(key));

  return { setSelectedPostalCode, selectedPostalCode };
};

export default selectedPostalCodeSlice.reducer;
