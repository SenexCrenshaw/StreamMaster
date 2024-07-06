import { GetStreamGroups } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamGroups = createAsyncThunk('cache/getGetStreamGroups', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStreamGroups');
    const response = await GetStreamGroups();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


