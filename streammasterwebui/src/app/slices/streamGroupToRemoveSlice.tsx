import { type PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

import { type RootState } from '../store';

type SetStreamGroupToRemoveSlicePayload = {
  toRemove: number;
  typename: string;
}

type QueryFilterState = Record<string, number | undefined>;

const initialState: QueryFilterState = {};

const streamGroupToRemoveSlice = createSlice({
  initialState,
  name: 'streamGroupToRemove',
  reducers: {
    setStreamGroupToRemove: (state, action: PayloadAction<SetStreamGroupToRemoveSlicePayload>) => {
      const { typename, toRemove } = action.payload;

      if (toRemove !== null && toRemove !== undefined) {
        state[typename] = toRemove;
      } else {

        delete state[typename]; // Remove the key if the filter is null or undefined
      }
    }
  }
});

export const selectStreamToRemove = (state: RootState, typename: number) => state.streamGroupToRemove[typename];
export const { setStreamGroupToRemove } = streamGroupToRemoveSlice.actions;
export default streamGroupToRemoveSlice.reducer;
