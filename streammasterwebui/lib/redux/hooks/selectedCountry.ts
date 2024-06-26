import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectedCountryPayload {
  value: string | null;
  key: string;
}

type SelectedCountryState = Record<string, string | null>;

const initialState: SelectedCountryState = {};

const selectedCountrySlice = createSlice({
  initialState,
  name: 'selectedCountry',
  reducers: {
    SetSelectedCountry: (state, action: PayloadAction<SetSelectedCountryPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectedCountry = createSelector(
  (state: RootState) => state.selectedCountry,
  (selectedCountry) => (key: string) => selectedCountry[key]
);

export const useSelectedCountry = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedCountry = (value: string | null) => {
    dispatch(
      selectedCountrySlice.actions.SetSelectedCountry({
        key,
        value
      })
    );
  };

  const selectedCountry = useSelector((state: RootState) => selectSelectedCountry(state)(key));

  return { setSelectedCountry, selectedCountry };
};

export default selectedCountrySlice.reducer;
