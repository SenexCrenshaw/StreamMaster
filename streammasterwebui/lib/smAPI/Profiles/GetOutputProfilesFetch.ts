import { GetOutputProfiles } from '@lib/smAPI/Profiles/ProfilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetOutputProfiles = createAsyncThunk('cache/getGetOutputProfiles', async (_: void, thunkAPI) => {
  try {
    const response = await GetOutputProfiles();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


