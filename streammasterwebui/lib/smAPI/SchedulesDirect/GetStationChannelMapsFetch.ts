import { GetStationChannelMaps } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStationChannelMaps = createAsyncThunk('cache/getGetStationChannelMaps', async (_: void, thunkAPI) => {
  try {
    const response = await GetStationChannelMaps();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


