import { GetIsSystemReady } from '@lib/smAPI/General/GeneralCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetIsSystemReady = createAsyncThunk('cache/getGetIsSystemReady', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetIsSystemReady');
    const response = await GetIsSystemReady();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


