import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';

import { ChannelGroupDto } from '@/lib/iptvApi';
import { type RootState } from '../../../lib/redux/store';

type selectedChannelGroupsSlicePayload = {
  ChannelGroupDtos: ChannelGroupDto[];
  typename: string;
};

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
        delete state[typename]; // Remove the key if the array is empty
      }
    },
  },
});

// Extract the selectedChannelGroups from the state
const selectVideoStreamsState = (state: RootState) => state.selectedChannelGroups;

// Pass-through function for typename

const passThrough = (_: any, typename: string) => typename;

export const makeselectedChannelGroups = createSelector(
  [selectVideoStreamsState, passThrough], // array of input selectors
  (selectedChannelGroups, typename) => selectedChannelGroups[typename] || [], // resulting selector
);

export const { setselectedChannelGroupsInternal } = selectedChannelGroupsSlice.actions;
export default selectedChannelGroupsSlice.reducer;
