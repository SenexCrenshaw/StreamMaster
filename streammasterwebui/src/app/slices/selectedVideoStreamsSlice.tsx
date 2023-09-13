import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';

import { type VideoStreamDto } from '../../store/iptvApi';
import { type RootState } from '../store';

type selectedVideoStreamsSlicePayload = {
  typename: string;
  videoStreamDtos: VideoStreamDto[];
}

type QueryFilterState = Record<string, VideoStreamDto[]>;

const initialState: QueryFilterState = {};

const selectedVideoStreamsSlice = createSlice({
  initialState,
  name: 'selectedVideoStreams',
  reducers: {
    setSelectedVideoStreamsInternal: (state, action: PayloadAction<selectedVideoStreamsSlicePayload>) => {
      const { typename, videoStreamDtos = [] } = action.payload; // Set the default value here

      if (videoStreamDtos.length > 0) {
        state[typename] = videoStreamDtos;
      } else {
        // eslint-disable-next-line @typescript-eslint/no-dynamic-delete
        delete state[typename]; // Remove the key if the array is empty
      }
    }
  }
});

// Extract the selectedVideoStreams from the state
const selectVideoStreamsState = (state: RootState) => state.selectedVideoStreams;

// Pass-through function for typename
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const passThrough = (_: any, typename: string) => typename;

export const makeSelectedVideoStreams = createSelector(
  [selectVideoStreamsState, passThrough], // array of input selectors
  (selectedVideoStreams, typename) => selectedVideoStreams[typename] || []// resulting selector
);

// export const selectedVideoStreams = (state: RootState, typename: number) => state.selectedVideoStreams[typename];
export const { setSelectedVideoStreamsInternal } = selectedVideoStreamsSlice.actions;
export default selectedVideoStreamsSlice.reducer;
