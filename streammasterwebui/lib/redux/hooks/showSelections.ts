import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetShowSelectionsPayload {
  value: boolean | null | undefined;
  key: string;
}

type ShowSelectionsState = Record<string, boolean | null | undefined>;

const initialState: ShowSelectionsState = {};

const showSelectionsSlice = createSlice({
  initialState,
  name: 'showSelections',
  reducers: {
    SetShowSelections: (state, action: PayloadAction<SetShowSelectionsPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectShowSelections = createSelector(
  (state: RootState) => state.showSelections,
  (showSelections) => (key: string) => showSelections[key]
);

export const useShowSelections = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setShowSelections = (value: boolean | null | undefined) => {
    dispatch(
      showSelectionsSlice.actions.SetShowSelections({
        key,
        value
      })
    );
  };

  const showSelections = useSelector((state: RootState) => selectShowSelections(state)(key));

  return { setShowSelections, showSelections };
};

export default showSelectionsSlice.reducer;
