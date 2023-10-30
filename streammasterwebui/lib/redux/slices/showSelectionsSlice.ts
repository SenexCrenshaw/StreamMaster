import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../store';

interface SetShowSelectionsPayload {
  selections: boolean | null | undefined;
  typename: string;
}

type ShowSelectionsState = Record<string, boolean | null | undefined>;

const initialState: ShowSelectionsState = {};

const showSelectionsSlice = createSlice({
  initialState,
  name: 'showSelections',
  reducers: {
    setShowSelectionsInternal: (state, action: PayloadAction<SetShowSelectionsPayload>) => {
      const { typename, selections } = action.payload;
      state[typename] = selections;
    }
  }
});

export const showSelections = (state: RootState, typename: string) => state.showSelections[typename];
export const { setShowSelectionsInternal } = showSelectionsSlice.actions;
export default showSelectionsSlice.reducer;
