import { GetStationChannelNames } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStationChannelNames = createAsyncThunk('cache/getGetStationChannelNames', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetStationChannelNames');
    const response = await GetStationChannelNames();
    console.log('Fetched GetStationChannelNames ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


