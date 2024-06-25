import { GetSubScribedHeadends } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetSubScribedHeadends = createAsyncThunk('cache/getGetSubScribedHeadends', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSubScribedHeadends');
    const response = await GetSubScribedHeadends();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


