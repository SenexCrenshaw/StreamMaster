import { type PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

import { type RootState } from '../store';

type SetStreamToRemoveSlicePayload = {
  toRemove: string;
  typename: string;
}

type QueryFilterState = Record<string, string | undefined>;

const initialState: QueryFilterState = {};

const streamToRemoveSlice = createSlice({
  initialState,
  name: 'streamToRemove',
  reducers: {
    setStreamToRemove: (state, action: PayloadAction<SetStreamToRemoveSlicePayload>) => {
      const { typename, toRemove } = action.payload;
      if (toRemove !== null && toRemove !== undefined) {
        state[typename] = toRemove;
      } else {
        // eslint-disable-next-line @typescript-eslint/no-dynamic-delete
        delete state[typename]; // Remove the key if the filter is null or undefined
      }
    }
  }
});

export const selectStreamToRemove = (state: RootState, typename: string) => state.streamToRemove[typename];
export const { setStreamToRemove } = streamToRemoveSlice.actions;
export default streamToRemoveSlice.reducer;
