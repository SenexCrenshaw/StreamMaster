import { GetSystemStatus } from '@lib/smAPI/General/GeneralCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetSystemStatus = createAsyncThunk('cache/getGetSystemStatus', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSystemStatus');
    const response = await GetSystemStatus();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


