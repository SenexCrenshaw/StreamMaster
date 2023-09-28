import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { type RootState } from '../../../lib/redux/store';

type SetShowHiddenPayload = {
  hidden: boolean | null | undefined,
  typename: string,
};

type ShowHiddenState = Record<string, boolean | null | undefined>;

const initialState: ShowHiddenState = {};

const showHiddenSlice = createSlice({
  initialState,
  name: 'showHidden',
  reducers: {
    setShowHiddenInternal: (state, action: PayloadAction<SetShowHiddenPayload>) => {
      const { typename, hidden } = action.payload;

      state[typename] = hidden;
    },
  },
});

export const showHidden = (state: RootState, typename: string) => state.showHidden[typename];
export const { setShowHiddenInternal } = showHiddenSlice.actions;
export default showHiddenSlice.reducer;
