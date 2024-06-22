import { GetLineupPreviewChannel } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetLineupPreviewChannelRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetLineupPreviewChannel = createAsyncThunk('cache/getGetLineupPreviewChannel', async (param: GetLineupPreviewChannelRequest, thunkAPI) => {
  try {
    const response = await GetLineupPreviewChannel(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


