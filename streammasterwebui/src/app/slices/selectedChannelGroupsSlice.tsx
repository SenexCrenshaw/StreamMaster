import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';

import { type ChannelGroupDto } from '../../store/iptvApi';
import { type RootState } from '../store';

type selectedChannelGroupsSlicePayload = {
  ChannelGroupDtos: ChannelGroupDto[];
  typename: string;
}

type QueryFilterState = Record<string, ChannelGroupDto[]>;

const initialState: QueryFilterState = {};

const selectedChannelGroupsSlice = createSlice({
  initialState,
  name: 'selectedChannelGroups',
  reducers: {
    setselectedChannelGroupsInternal: (state, action: PayloadAction<selectedChannelGroupsSlicePayload>) => {
      const { typename, ChannelGroupDtos = [] } = action.payload; // Set the default value here
      if (ChannelGroupDtos.length > 0) {
        state[typename] = ChannelGroupDtos;
      } else {
        // eslint-disable-next-line @typescript-eslint/no-dynamic-delete
        delete state[typename]; // Remove the key if the array is empty
      }
    }
  }
});

// Extract the selectedChannelGroups from the state
const selectVideoStreamsState = (state: RootState) => state.selectedChannelGroups;

// Pass-through function for typename
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const passThrough = (_: any, typename: string) => typename;

export const makeselectedChannelGroups = createSelector(
  [selectVideoStreamsState, passThrough], // array of input selectors
  (selectedChannelGroups, typename) => selectedChannelGroups[typename] || []// resulting selector
);

// export const selectedChannelGroups = (state: RootState, typename: number) => state.selectedChannelGroups[typename];
export const { setselectedChannelGroupsInternal } = selectedChannelGroupsSlice.actions;
export default selectedChannelGroupsSlice.reducer;
