import { createSlice, type PayloadAction } from '@reduxjs/toolkit';

import { type RootState } from '../store';

type SetChannelGroupToRemoveSlicePayload = {
  toRemove: number,
  typename: string,
};

type QueryFilterState = Record<string, number | undefined>;

const initialState: QueryFilterState = {};

const channelGroupToRemoveSlice = createSlice({
  initialState,
  name: 'channelGroupToRemove',
  reducers: {
    setChannelGroupToRemove: (state, action: PayloadAction<SetChannelGroupToRemoveSlicePayload>) => {
      const { typename, toRemove } = action.payload;

      if (toRemove !== null && toRemove !== undefined) {
        state[typename] = toRemove;
      } else {

        // eslint-disable-next-line @typescript-eslint/no-dynamic-delete
        delete state[typename]; // Remove the key if the filter is null or undefined
      }
    },
  },
});

export const selectStreamToRemove = (state: RootState, typename: number) => state.channelGroupToRemove[typename];
export const { setChannelGroupToRemove } = channelGroupToRemoveSlice.actions;
export default channelGroupToRemoveSlice.reducer;
