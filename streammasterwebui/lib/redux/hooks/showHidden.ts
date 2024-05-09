import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetShowHiddenPayload {
  value: boolean | null;
  key: string;
}

type ShowHiddenState = Record<string, boolean | null>;

const initialState: ShowHiddenState = {};

const showHiddenSlice = createSlice({
  initialState,
  name: 'showHidden',
  reducers: {
    SetShowHidden: (state, action: PayloadAction<SetShowHiddenPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectShowHidden = createSelector(
  (state: RootState) => state.showHidden,
  (showHidden) => (key: string) => showHidden[key]
);

export const useShowHidden = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setShowHidden = (value: boolean | null) => {
    dispatch(
      showHiddenSlice.actions.SetShowHidden({
        key,
        value
      })
    );
  };

  const showHidden = useSelector((state: RootState) => selectShowHidden(state)(key));

  return { setShowHidden, showHidden };
};

export default showHiddenSlice.reducer;
