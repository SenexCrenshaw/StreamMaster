import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetShowSelectedPayload {
  value: boolean | null;
  key: string;
}

type ShowSelectedState = Record<string, boolean | null>;

const initialState: ShowSelectedState = {};

const showSelectedSlice = createSlice({
  initialState,
  name: 'showSelected',
  reducers: {
    SetShowSelected: (state, action: PayloadAction<SetShowSelectedPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectShowSelected = createSelector(
  (state: RootState) => state.showSelected,
  (showSelected) => (key: string) => showSelected[key]
);

export const useShowSelected = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setShowSelected = (value: boolean | null) => {
    dispatch(
      showSelectedSlice.actions.SetShowSelected({
        key,
        value
      })
    );
  };

  const showSelected = useSelector((state: RootState) => selectShowSelected(state)(key));

  return { setShowSelected, showSelected };
};

export default showSelectedSlice.reducer;
